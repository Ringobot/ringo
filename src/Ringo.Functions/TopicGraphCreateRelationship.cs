using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Ringo.Common.Models;

namespace Ringo.Functions
{
    public static class TopicGraphCreateRelationship
    {
        [FunctionName("TopicGraphCreateRelationship")]
        public static async Task Run([ServiceBusTrigger("graph", "addRelationship")]string msg, TraceWriter log)
        {
            if (log != null) log.Info(msg);
            GremlinRelationship input = JsonConvert.DeserializeObject<GremlinRelationship>(msg);

            await GraphHelper.CreateRelationship(input);
        }
    }
}
