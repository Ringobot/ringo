// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGridExtensionConfig?functionName={functionname}

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.WebJobs.Extensions.CosmosDB;

namespace Ringo.Functions
{
    public static class EventLogging
    {
        [FunctionName("EventLogging")]
        public static void Run([EventGridTrigger]EventGridEvent eventGridEvent,
            [CosmosDB(
                databaseName: "ringo",
                collectionName: "logs",
                ConnectionStringSetting = "CosmosDBLogging")]out dynamic document,
            TraceWriter log)
        {
            log.Info(eventGridEvent.Data.ToString());
            document = eventGridEvent;
        }
    }
}
