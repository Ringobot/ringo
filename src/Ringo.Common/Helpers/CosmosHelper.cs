using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Ringo.Common.Models;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Ringo.Common.Heplers
{
    public static class CosmosHelper
    {
        private static readonly string endpointUrl = Environment.GetEnvironmentVariable("CosmosEndpoint");
        private static readonly string authorizationKey = Environment.GetEnvironmentVariable("CosmosKey");
        private static readonly string databaseId = Environment.GetEnvironmentVariable("CosmosDB");
        private static readonly string collectionId = Environment.GetEnvironmentVariable("CosmosCollection");
        private static DocumentClient documentClient = new DocumentClient(new Uri(endpointUrl), authorizationKey);


        public static async Task<dynamic> CreateDocument(Entity entity)
        {
            dynamic doc = entity.Properties;
            doc.Add("id", entity.Id);
            doc.Add("name", entity.Name);
            doc.Add("createdate", entity.CreateDate);

            var response = await documentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseId, collectionId), doc);
            return response;
        }

        public static async Task<dynamic> RemoveVertex(string vertexId)
        {
            return await documentClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId, collectionId, vertexId));
        }

        public static async Task CreateDocuments(EntityRelationship input)
        {
            await CheckOrCreateDocument(input.FromVertex);
            await CheckOrCreateDocument(input.ToVertex);

        }

        private static async Task CheckOrCreateDocument(Entity entity)
        {
            try
            {
                var response = await documentClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseId, collectionId, entity.Id));
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    await CreateDocument(entity);
                }
                else
                {
                    throw;
                }
            }
        }

        private static async Task<DocumentCollection> GetOrCreateCollectionAsync(string databaseId, string collectionId)
        {
            var databaseUri = UriFactory.CreateDatabaseUri(databaseId);

            DocumentCollection collection = documentClient.CreateDocumentCollectionQuery(databaseUri)
                .Where(c => c.Id == collectionId)
                .AsEnumerable()
                .FirstOrDefault();

            if (collection == null)
            {
                collection = await documentClient.CreateDocumentCollectionAsync(databaseUri, new DocumentCollection { Id = collectionId });
            }

            return collection;
        }
    }
}