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
        [HttpGet("{artist}")]
        public async Task<Artist> GetArtist(string artist)
        {

            Console.WriteLine();
            var result = await ((IArtistService)artistService).GetArtist(artist);

            return result;
        }

        public Task<Artist> GetArtistByUri(string artistUri)
        {
            return ((IArtistService)artistService).GetArtistByUri(artistUri);
        }

        public Task<List<Artist>> GetRelatedArtists(string artist)
        {
            return ((IArtistService)artistService).GetRelatedArtists(artist);
        }

        public Task<List<Artist>> SearchArtists(string artist, int limit = 3)
        {
            return ((IArtistService)artistService).SearchArtists(artist, limit);
        }

        public List<EntityRelationship> PushRelatedArtist(Artist baseArtist, List<Artist> relatedArtists)
        {
            return ((IArtistService)artistService).PushRelatedArtist(baseArtist, relatedArtists);
        }
    }
}
