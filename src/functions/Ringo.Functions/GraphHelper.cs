using System;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using Ringo.Common.Models;
using Microsoft.Azure.Graphs;
using Microsoft.Azure.Documents.Linq;
using System.Linq;

namespace Ringo.Functions
{
    public static class GraphHelper
    {
        private static readonly string endpointUrl = Environment.GetEnvironmentVariable("CosmosGraphEndpoint");
        private static readonly string authorizationKey = Environment.GetEnvironmentVariable("CosmosGraphKey");
        private static readonly string databaseId = Environment.GetEnvironmentVariable("CosmosGraphDB");
        private static readonly string collectionId = Environment.GetEnvironmentVariable("CosmosGraphCollection");
        private static DocumentClient documentClient = new DocumentClient(new Uri(endpointUrl), authorizationKey);

        public static async Task CreateVertex(string vertexId)
        {
            var collectionLink = await GetOrCreateCollectionAsync(databaseId, collectionId);
            var gremlinQuery = $@"g.addV('entity').property('id', '{vertexId}')";

            IDocumentQuery<dynamic> query = documentClient.CreateGremlinQuery<dynamic>(collectionLink, gremlinQuery);
            dynamic result = await query.ExecuteNextAsync();
        }

        public static async Task<HttpStatusCode> RemoveVertex(string vertexId)
        {
            return (await documentClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId, collectionId, vertexId))).StatusCode;
        }

        public static async Task CreateRelationship(GremlinRelationship input)
        {
            var collectionLink = await GetOrCreateCollectionAsync(databaseId, collectionId);
            var gremlinQuery = $@"g.V('{input.FromVertex}').addE('{input.Relationship}').to(g.V('{input.ToVertex}'))";

            IDocumentQuery<dynamic> query = documentClient.CreateGremlinQuery<dynamic>(collectionLink, gremlinQuery);
            dynamic result = await query.ExecuteNextAsync();

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