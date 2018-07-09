using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Ringo.Common.Services;

namespace Ringo.SF.ArtistStateless.Controllers
{
    [Route("api/[controller]")]
    public class ArtistController : Controller
    {
        private ArtistService artistService;
        private readonly string spotifyApiClientId;
        private IConfiguration Configuration;

        public ArtistController(IConfiguration configuration)
        {
            artistService = new ArtistService();
            this.spotifyApiClientId = Environment.GetEnvironmentVariable("SpotifyApiClientId");
            Configuration = configuration;
        }

        // GET api/values/5
        [HttpGet("{artist}")]
        public async Task<ActionResult> Get(string artist)
        {
            var testEnvId = Environment.GetEnvironmentVariable("SpotifyApiClientId");
            var testEnvSec = Environment.GetEnvironmentVariable("SpotifyApiClientSecret");

            Console.WriteLine();
            var result = await artistService.GetArtist(artist);
            return (ActionResult)new OkObjectResult(result);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
