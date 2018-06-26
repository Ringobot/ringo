using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Ringo.Common.Services;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ringo.Functions
{
    public static class FindArtistMatch
    {
        [FunctionName("FindArtistMatch")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            dynamic data = await req.Content.ReadAsAsync<object>();

            ArtistService artistService = new ArtistService();
            var result = await artistService.FindArtistMatch(data.artistId.ToString());

            return result.Count != 0
                ? (ActionResult)new OkObjectResult(result)
                : new BadRequestObjectResult("Unable to find artist");
        }
    }
}
