using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Ringo.Common.Models;
using Ringo.Common.Services;

namespace Ringo.SF.ArtistStateless.Controllers
{
    [Route("api/[controller]")]
    public class ArtistController : Controller, IArtistService
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
            return await ((IArtistService)artistService).GetArtist(artist);
        }

        [HttpGet("uri")]
        public async Task<Artist> GetArtistByUri([FromQuery(Name = "artist")]string artistUri)
        {
            return await ((IArtistService)artistService).GetArtistByUri(artistUri);
        }

        [HttpGet("related")]
        public async Task<List<Artist>> GetRelatedArtists([FromQuery(Name = "artist")]string artist)
        {
            return await ((IArtistService)artistService).GetRelatedArtists(artist);
        }

        [HttpGet("search")]
        public async Task<List<Artist>> SearchArtists([FromQuery(Name = "artist")]string artist, int limit = 3)
        {
            return await ((IArtistService)artistService).SearchArtists(artist, limit);
        }

        public List<EntityRelationship> PushRelatedArtist(Artist baseArtist, List<Artist> relatedArtists)
        {
            return ((IArtistService)artistService).PushRelatedArtist(baseArtist, relatedArtists);
        }
    }
}
