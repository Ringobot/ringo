using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Ringo.Common.Services;
using System.Threading.Tasks;

namespace Ringo.Functions
{
    public static class GetArtist
    {
        [FunctionName("GetArtist")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Admin, "get", Route = "artist/{type}")]HttpRequest req, string type, TraceWriter log)
        {
            if (log != null) log.Info($"Processed request for {type}");
            string artist = req.Query["artist"];
            if (log != null) log.Info($"Processed request for {artist}");

            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            ArtistService artistService = new ArtistService(config);

            switch (type)
            {
                case "id":
                    {
                        var result = await artistService.GetArtist(artist);
                        return (ActionResult)new OkObjectResult(result);
                    }
                case "uri":
                    {
                        var result = await artistService.GetArtistByUri(artist);
                        return (ActionResult)new OkObjectResult(result);
                    }
                case "related":
                    {
                        var result = await artistService.GetRelatedArtists(artist);
                        return (ActionResult)new OkObjectResult(result);
                    }
                case "search":
                    {
                        var result = await artistService.SearchArtists(artist);
                        return (ActionResult)new OkObjectResult(result);
                    }
                default:
                    {
                        return (ActionResult)new BadRequestObjectResult("Unable to find artist");
                    }
            }
        }
    }
}
