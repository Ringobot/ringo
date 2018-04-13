using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Ringo.Common.Models;

namespace Ringo.Functions
{
    public static class TopicGraphCreateEntity
    {
        [FunctionName("TopicGraphCreateEntity")]
        public static async Task Run([ServiceBusTrigger("graph", "addEntity")]string msg, TraceWriter log)
        {
            if (log != null) log.Info(msg);
            Entity input = JsonConvert.DeserializeObject<Entity>(msg);
            await GraphHelper.CreateVertex(input);
        }
    }
}
