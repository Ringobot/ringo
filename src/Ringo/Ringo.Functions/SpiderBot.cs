using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Bot.Connector.DirectLine;

namespace Ringo.Functions
{
    public static class SpiderBot
    {


        [FunctionName("SpiderBot")]
        public static async Task Run([TimerTrigger("10 * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            if (log != null) log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

            await BotHelper.BuildArtistGraph();

        }
    }

}
