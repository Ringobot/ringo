using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Ringo.Common.Models;
using Ringo.Common.Heplers;

namespace Ringo.Functions
{
    public static class TopicCosmosCreate
    {
        [FunctionName("TopicCosmosCreate")]
        public static async Task Run([ServiceBusTrigger("graph", "addDBEntity")]string msg, TraceWriter log)
        {
            if (log != null) log.Info(msg);
            EntityRelationship msgObj = JsonConvert.DeserializeObject<EntityRelationship>(msg);
            await CosmosHelper.CreateDocuments(msgObj);
        }
    }
}
