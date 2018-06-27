//using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Host;
//using Newtonsoft.Json;
//using Ringo.Common.Heplers;
//using Ringo.Common.Models;
//using System.Threading.Tasks;

//namespace Ringo.Functions
//{
//    public static class TopicGraphCreateRelationship
//    {
//        [FunctionName("TopicGraphCreateRelationship")]
//        public static async Task Run([ServiceBusTrigger("graph", "addRelationship")]string msg, TraceWriter log)
//        {
//            if (log != null) log.Info(msg);
//            EntityRelationship msgObj = JsonConvert.DeserializeObject<EntityRelationship>(msg);
//            await GraphHelper.CreateRelationship(msgObj);
//        }
//    }
//}
