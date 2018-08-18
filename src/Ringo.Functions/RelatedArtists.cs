using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Ringo.Common.Helpers;
using Ringo.Common.Models;
using Ringo.Common.Services;

namespace Ringo.Functions.Artists
{
    public static class RelatedArtists
    {
        [FunctionName("RelatedArtists_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
            [OrchestrationClient]DurableOrchestrationClient starter,
            TraceWriter log)
        {
            // Function input comes from the request content.
            dynamic eventData = await req.Content.ReadAsAsync<object>();
            string instanceId = await starter.StartNewAsync("RelatedArtists", eventData);

            log.Info($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        [FunctionName("RelatedArtists")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            var outputs = new List<string>();
        
            dynamic entity = context.GetInput<object>();
            List<EntityRelationship> msgObj = JsonConvert.DeserializeObject<List<EntityRelationship>>(entity.ToString());

            foreach (EntityRelationship er in msgObj)
            {
                if (er.FromVertex.Properties["type"].ToString() != "user")
                {
                    var degree = await context.CallActivityAsync<List<EntityRelationship>>("RelatedArtists_GetRelationship", er.ToVertex.Properties["spotifyid"].ToString());
                    foreach (var item in degree)
                    {
                        await context.CallActivityAsync<string>("RelatedArtists_CreateRelationship", item);
                    }
                }

            }


            foreach (EntityRelationship er in msgObj)
            {
                outputs.Add(await context.CallActivityAsync<string>("RelatedArtists_CreateRelationship", er));
            }

            return outputs;
        }

        [FunctionName("RelatedArtists_GetRelationship")]
        public static async Task<List<EntityRelationship>> GetRelationship([ActivityTrigger] string artistId, TraceWriter log)
        {
            List<Artist> artists = new List<Artist>();
            List<EntityRelationship> related2degree = new List<EntityRelationship>();

            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            ArtistService artistService = new ArtistService(config);

            artists = await artistService.GetRelatedArtists(artistId);
            related2degree.AddRange(await artistService.PushRelatedArtist(artistId, artists));

            return related2degree;

        }


        [FunctionName("RelatedArtists_CreateRelationship")]
        public static async Task<string> CreateRelationship([ActivityTrigger] EntityRelationship entityRelationship, TraceWriter log)
        {
            await GraphHelper.CreateRelationship(entityRelationship);
            log.Info($"Added: {entityRelationship.FromVertex.Name}, {entityRelationship.Relationship}, To: {entityRelationship.ToVertex.Name}");
            return $"Added: {entityRelationship.FromVertex.Name}, {entityRelationship.Relationship}, To: {entityRelationship.ToVertex.Name}";
        }


    }
}