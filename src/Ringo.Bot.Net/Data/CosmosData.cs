using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RingoBotNet.Helpers;
using RingoBotNet.Models;
using System;
using System.Threading.Tasks;

namespace RingoBotNet.Data
{
    public abstract class CosmosData
    {
        //protected readonly IConfiguration _config;
        protected readonly ILogger _logger;
        private readonly IDocumentClient _client;
        private readonly string _databaseName;
        private readonly string _collectionName;
        protected readonly Uri _collectionUri;

        public CosmosData(
            IConfiguration configuration,
            ILogger logger,
            string collectionName)
        {
            _logger = logger;
            _databaseName = configuration[ConfigHelper.CosmosDBDatabaseName];
            _collectionName = collectionName;
            _collectionUri = UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName);

            _client = new DocumentClient(
                        new Uri(configuration[ConfigHelper.CosmosDBEndpoint]),
                        configuration[ConfigHelper.CosmosDBPrimaryKey]);
        }

        protected async Task Create(CosmosEntity document) //where T : CosmosEntity
        {
            document.EnforceInvariants();
            var response = await _client.CreateDocumentAsync(
                _collectionUri,
                document,
                options: new RequestOptions { PartitionKey = new PartitionKey(document.PartitionKey) },
                disableAutomaticIdGeneration: true);
            _logger.LogDebug($"Create: Type = {document.Type} id = \"{document.Id}\" PK = \"{document.PartitionKey}\" RequestCharge = {response.RequestCharge}");
        }

        protected async Task Upsert(CosmosEntity document)
        {
            document.EnforceInvariants();
            // last write wins
            var response = await _client.UpsertDocumentAsync(
                _collectionUri,
                document,
                options: new RequestOptions { PartitionKey = new PartitionKey(document.PartitionKey) },
                disableAutomaticIdGeneration: true);
            _logger.LogDebug($"Upsert: Type = {document.Type} id = \"{document.Id}\" PK = \"{document.PartitionKey}\" RequestCharge = {response.RequestCharge}");
        }

        protected internal virtual async Task Replace(CosmosEntity document)
        {
            document.EnforceInvariants();
            // last write wins
            var response = await _client.ReplaceDocumentAsync(
                UriFactory.CreateDocumentUri(_databaseName, _collectionName, document.Id),
                document,
                options: new RequestOptions { PartitionKey = new PartitionKey(document.PartitionKey) });
            _logger.LogDebug($"Replace: Type = {document.Type} id = \"{document.Id}\" PK = \"{document.PartitionKey}\" RequestCharge = {response.RequestCharge}");
        }

        /// <summary>
        /// Reads a document
        /// </summary>
        /// <typeparam name="T">The type of the Document</typeparam>
        /// <param name="id">The document Id</param>
        /// <param name="partitionKey">Optional. A partition key for the document. If null, Id will be used.</param>
        /// <returns>The document as T</returns>
        protected internal virtual async Task<T> Read<T>((string id, string pk) idPK) where T : class
        {
            try
            {
                DocumentResponse<T> documentResponse = await _client.ReadDocumentAsync<T>(
                               UriFactory.CreateDocumentUri(_databaseName, _collectionName, idPK.id),
                               options: new RequestOptions { PartitionKey = new PartitionKey(idPK.pk) });
                _logger.LogDebug($"Read: {documentResponse.Document.GetType().Name} idPK = \"{idPK}\" RequestCharge = {documentResponse.RequestCharge}");

                return documentResponse.Document;
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Logger.Error(ex.Message, ex, nameof(CosmosData));
                    Logger.Information($"Document not found. Returning null. idPK = \"{idPK}\".");
                    return null;
                }

                throw;
            }
        }
    }
}
