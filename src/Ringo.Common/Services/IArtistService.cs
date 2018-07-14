using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ringo.Common.Models;

namespace Ringo.Common.Services
{
    public interface IArtistService
    {
        Task<Artist> GetArtist(string artistId);

        Task<Artist> GetArtistByUri(string artistUri);

        Task<List<Artist>> GetRelatedArtists(string artist);

        Task<List<Artist>> SearchArtists(string artist, int limit = 3);

        //List<EntityRelationship> PushRelatedArtist(Artist baseArtist, List<Artist> relatedArtists);

    }
}
