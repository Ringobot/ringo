using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Ringo.Common.Services;
using Ringo.Common.Models;

namespace Ringo.SF.StatefulBackend.Controllers
{
    [Route("api/[controller]")]
    public class ArtistController : Controller
    {
        private ArtistService artistService;
        private IConfiguration Configuration;

        public ArtistController(IConfiguration configuration)
        {
            artistService = new ArtistService(configuration);
            Configuration = configuration;

        }

        // GET api/values/5
        [HttpGet("id")]
        public async Task<Artist> GetArtist([FromQuery(Name = "artist")]string artist)
        {
            return await artistService.GetArtist(artist);
        }

        [HttpGet("uri")]
        public async Task<Artist> GetArtistByUri([FromQuery(Name = "artist")]string artistUri)
        {
            return await artistService.GetArtistByUri(artistUri);
        }

        [HttpGet("related")]
        public async Task<List<Artist>> GetRelatedArtists([FromQuery(Name = "artist")]string artist)
        {
            var related = await artistService.GetRelatedArtists(artist);
            PushRelatedArtist(artist, related);
            return related;
        }

        [HttpGet("search")]
        public async Task<List<Artist>> SearchArtists([FromQuery(Name = "artist")]string artist, int limit = 3)
        {
            return await artistService.SearchArtists(artist, limit);
        }

        public void PushRelatedArtist(string baseArtist, List<Artist> relatedArtists)
        {

        }

    }
}
