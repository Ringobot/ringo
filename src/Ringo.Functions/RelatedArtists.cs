using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Ringo.Common.Heplers;
using Ringo.Common.Models;

namespace Ringo.Functions.Artists
{
    public static class RelatedArtists
    {
        [FunctionName("RelatedArtists")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            var outputs = new List<string>();
            dynamic entity = context.GetInput<object>();
            List<EntityRelationship> msgObj = JsonConvert.DeserializeObject<List<EntityRelationship>>(entity.ToString());

            foreach (EntityRelationship er in msgObj)
            {
                outputs.Add(await context.CallActivityAsync<string>("RelatedArtists_CreateRelationship", er));
            }

            return outputs;
        }

        [FunctionName("RelatedArtists_CreateRelationship")]
        public static async Task<string> CreateRelationship([ActivityTrigger] EntityRelationship entityRelationship, TraceWriter log)
        {
            await GraphHelper.CreateRelationship(entityRelationship);
            return $"Added: {entityRelationship.FromVertex.Name}, {entityRelationship.Relationship}, To: {entityRelationship.ToVertex.Name}";
        }

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
    }
}