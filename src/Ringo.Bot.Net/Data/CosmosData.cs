using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using RingoBotNet.Models;
using System;
using System.Threading.Tasks;

namespace RingoBotNet.Data
{
    public abstract class CosmosData
    {
        protected readonly IConfiguration _config;
        protected readonly IDocumentClient _client;
        protected readonly string _databaseName;
        protected readonly string _collectionName;
        protected readonly Uri _collectionUri;

        public CosmosData(IConfiguration configuration,
            IDocumentClient documentClient,
            string databaseName,
            string collectionName)
        {
            _config = configuration;
            _client = documentClient;
            _databaseName = databaseName;
            _collectionName = collectionName;
            _collectionUri = UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName);
        }

        protected async Task<T> Create<T>(T document) where T:CosmosDocument
        {
            document.EnforceInvariants();
            return await _client.CreateDocumentAsync(
                _collectionUri, 
                document, 
                options: new RequestOptions { PartitionKey = new PartitionKey(document.PartitionKey) },
                disableAutomaticIdGeneration: true) as T;
        }

        protected async Task Upsert(CosmosDocument document)
        {
            document.EnforceInvariants();
            // last write wins
            await _client.UpsertDocumentAsync(
                _collectionUri, 
                document, 
                options: new RequestOptions { PartitionKey = new PartitionKey(document.PartitionKey) }, 
                disableAutomaticIdGeneration: true);
        }

        protected async Task Replace(CosmosDocument document)
        {
            document.EnforceInvariants();
            // last write wins
            await _client.ReplaceDocumentAsync(
                UriFactory.CreateDocumentUri(_databaseName, _collectionName, document.Id),
                document,
                options: new RequestOptions { PartitionKey = new PartitionKey(document.PartitionKey) });
        }

        /// <summary>
        /// Reads a document
        /// </summary>
        /// <typeparam name="T">The type of the Document</typeparam>
        /// <param name="id">The document Id</param>
        /// <param name="partitionKey">Optional. A partition key for the document. If null, Id will be used.</param>
        /// <returns>The document as T</returns>
        protected async Task<T> Read<T>(string id, string partitionKey = null) where T:class
        {
            try
            {
                DocumentResponse<T> documentResponse = await _client.ReadDocumentAsync<T>(
                               UriFactory.CreateDocumentUri(_databaseName, _collectionName, id),
                               options: new RequestOptions { PartitionKey = new PartitionKey(partitionKey ?? id) });
                return documentResponse.Document;
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Logger.Error(ex.Message, ex, nameof(CosmosData));
                    Logger.Information(
                        $"Document not found. Returning null. id = \"{id}\" partition key = \"{partitionKey}\".", 
                        nameof(CosmosData));
                    return null;
                }

                throw;
            }
        }
    }
}
