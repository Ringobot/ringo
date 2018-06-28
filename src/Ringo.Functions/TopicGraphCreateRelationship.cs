using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Ringo.Common.Heplers;
using Ringo.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ringo.Functions
{
    public static class TopicGraphCreateRelationship
    {
        [FunctionName("TopicGraphCreateRelationship")]
        public static async Task Run([ServiceBusTrigger("graph", "addRelationship", Connection = "ServiceBus")]string msg, TraceWriter log)
        {
            if (log != null) log.Info(msg);
            List<EntityRelationship> msgList = JsonConvert.DeserializeObject<List<EntityRelationship>>(msg);
            foreach (EntityRelationship entityRelationship in msgList)
            {
                await GraphHelper.CreateRelationship(entityRelationship);
            }
            
        }
    }
}
