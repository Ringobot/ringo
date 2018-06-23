using Ringo.Common.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using SpotifyApiDotNetCore;


namespace Ringo.Common.Heplers
{

    public class SpotifyService : ISpotifyWebApi
    {
        public Task<dynamic> GetArtist(string artistId)
        {
            return SpotifyHelperv1.FakeArtist();
        }

        public Task<dynamic> GetPlaylists(string username)
        {
            throw new NotImplementedException();
        }

        public Task<dynamic> GetPlaylists(string username, int offset)
        {
            throw new NotImplementedException();
        }

        public Task<dynamic> GetRecommendation(string artistSeed)
        {
            throw new NotImplementedException();
        }

        public Task<dynamic> GetRecommendation(string artistSeed, int limit)
        {
            throw new NotImplementedException();
        }

        public Task<dynamic> GetRelatedArtists(string artistId)
        {
            throw new NotImplementedException();
        }

        public Task PlayArtist(string userHash, string spotifyUri)
        {
            throw new NotImplementedException();
        }

        public Task<dynamic> SearchArtists(string artist)
        {
            throw new NotImplementedException();
        }

        public Task<dynamic> SearchArtists(string artist, int limit)
        {
            throw new NotImplementedException();
        }
    }

}
