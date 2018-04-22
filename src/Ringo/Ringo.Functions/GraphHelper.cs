﻿using System;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using Ringo.Common.Models;
using Microsoft.Azure.Graphs;
using Microsoft.Azure.Documents.Linq;
using System.Linq;
using Microsoft.Azure.Graphs.Elements;
using System.Text;
using System.Collections.Generic;

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
            var gremlinBaseQuery = $@"g.addV(T.Id, '{entity.Id}').property('name', '{entity.Name}')";
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
                var gremlinQuery = $@"g.V('{input.FromVertex.Id}').outE('likes').inV().has('id', '{input.ToVertex.Id}')";
                var likes = await RunDocumentQuery(gremlinQuery.ToString());
                if (likes.Count == 0)
                {
                    var gremlinInsert = $@"g.V('{input.FromVertex.Id}').addE('{input.Relationship}').to(g.V('{input.ToVertex.Id}'))";
                    await RunDocumentQuery(gremlinInsert.ToString());
                }

            }
            catch (DocumentClientException ex)
            {
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