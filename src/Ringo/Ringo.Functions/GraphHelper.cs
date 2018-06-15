using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Graphs;
using Newtonsoft.Json.Linq;
using Ringo.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ringo.Functions
{
    public static class GraphHelper
    {
        private static readonly string endpointUrl = Environment.GetEnvironmentVariable("CosmosGraphEndpoint");
        private static readonly string authorizationKey = Environment.GetEnvironmentVariable("CosmosGraphKey");
        private static readonly string databaseId = Environment.GetEnvironmentVariable("CosmosGraphDB");
        private static readonly string collectionId = Environment.GetEnvironmentVariable("CosmosGraphCollection");
        private static DocumentClient documentClient = new DocumentClient(new Uri(endpointUrl), authorizationKey);

        public static async Task<dynamic> GetVertex(string vertexId)
        {
            var gremlinQuery = ($@"g.V('{vertexId}')");
            return await RunDocumentQuery(gremlinQuery);
        }

        public static async Task<dynamic> CreateVertex(Entity entity)
        {
            entity.Name = entity.Name.Replace("'", @"\'");
            var gremlinBaseQuery = $"g.addV('{entity.Properties["type"]}').property(T.id, '{entity.Id}').property('name', '{entity.Name}')";
            StringBuilder gremlinQuery = new StringBuilder(gremlinBaseQuery);
            foreach (var p in entity.Properties)
            {
                gremlinQuery.Append($@".property('{p.Key}', '{p.Value}')");
            }

            return await RunDocumentQuery(gremlinQuery.ToString());
        }

        public static async Task<dynamic> RemoveVertex(string vertexId)
        {
            var gremlinQuery = ($@"g.V('{vertexId}').drop()");
            return await RunDocumentQuery(gremlinQuery);
        }

        public static async Task CreateRelationship(EntityRelationship input)
        {
            await CheckOrCreateDocument(input.FromVertex);
            await CheckOrCreateDocument(input.ToVertex);
            await CheckOrCreateRelationship(input);

        }

        public static async Task<List<string>> GetArtistRelatedLessThanTwo()
        {
            var gremlinQuery = ($@"g.V().out('related').where(__.out().count().is(lt(2))).project('spotifyid')");
            try
            {
                List<string> result = new List<string>();
                var collectionLink = await GetOrCreateCollectionAsync(databaseId, collectionId);
                IDocumentQuery<dynamic> query = documentClient.CreateGremlinQuery<dynamic>(collectionLink, gremlinQuery);
                foreach (JObject item in await query.ExecuteNextAsync<dynamic>())
                {
                    result.Add(item["spotifyid"]["id"].ToString());
                }
                return result;

                }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
           

        }

        private static async Task<dynamic> RunDocumentQuery(string gremlinQuery)
        {
            try
            {
                var collectionLink = await GetOrCreateCollectionAsync(databaseId, collectionId);
                IDocumentQuery<dynamic> query = documentClient.CreateGremlinQuery<dynamic>(collectionLink, gremlinQuery);
                dynamic result = await query.ExecuteNextAsync();
                return result;
            }
            catch (DocumentClientException ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

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
                    await CreateVertex(entity);
                }
                else
                {
                    throw;
                }
            }
        }

        private static async Task CheckOrCreateRelationship(EntityRelationship input)
        {
            try
            {
                //g.V('default-user').outE('likes').inV().has('id', 'metallica:c6b5b6413f293fce96539c41e704b5a2')
                var gremlinQuery = $@"g.V('{input.FromVertex.Id}').outE('{input.Relationship}').inV().has('id', '{input.ToVertex.Id}')";
                var likes = await RunDocumentQuery(gremlinQuery.ToString());
                if (likes.Count == 0)
                {
                    var gremlinInsert = $@"g.V('{input.FromVertex.Id}').addE('{input.Relationship}').to(g.V('{input.ToVertex.Id}'))";
                    await RunDocumentQuery(gremlinInsert.ToString());
                }

            }
            catch (DocumentClientException ex)
            {
                Console.WriteLine(ex.Message);
                throw;
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