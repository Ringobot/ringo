using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ringo.Common.Models;

namespace Ringo.Common.Services
{
    public interface IArtistService
    {
        Artists MapToArtist(string data);

        Task<Artist> GetArtist(string artistId);

        Task<Artist> GetArtistByUriAsync(string artistUri);

        bool FindArtistMatch(Artist artist);

        Task<Artists> SearchArtists(string artist, int limit = 3);

        Task<Artists> GetRelatedArtistsAsync(string artist);

        List<EntityRelationship> PushRelatedArtist(Artist baseArtist, Artists relatedArtists);

    }
}
