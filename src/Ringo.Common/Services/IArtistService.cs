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

        string GetArtist(string artistId);

        Artist GetArtistByUri(string artistUri);

        bool FindArtistMatch(Artist artist);

        Artists SearchArtists(string artist, int limit = 3);

        Artists GetRelatedArtists(string artist);

        List<EntityRelationship> PushRelatedArtist(Artist baseArtist, Artists relatedArtists);

    }
}
