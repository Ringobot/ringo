using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ringo.Common.Models;

namespace Ringo.Common.Services
{
    public interface IArtistService
    {
        List<Artist> MapToArtist(List<dynamic> data);

        Task<List<Artist>> GetArtist(string artistId);

        Task<List<Artist>> GetArtistByUriAsync(string artistUri);

        Task<Tuple<bool, List<Artist>>> FindArtistMatch(string artist);

        Task<List<Artist>> SearchArtists(string artist, int limit = 3);

        Task<List<Artist>> GetRelatedArtistsAsync(string artist);

        List<EntityRelationship> PushRelatedArtist(Artist baseArtist, List<Artist> relatedArtists);

    }
}
