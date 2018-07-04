using System;
using System.Net.Http;
using System.Threading.Tasks;
using SpotifyApi.NetCore;
using System.Collections.Generic;

namespace Ringo.Common.Services
{

    public class SpotifyService
    {
        private static HttpClient httpClient = new HttpClient();
        private static ClientCredentialsAuthorizationApi auth = new ClientCredentialsAuthorizationApi(httpClient);
        private static ArtistsApi api = new ArtistsApi(httpClient, auth);

        public async Task<dynamic> GetArtist(string artistId)
        {
            return await api.GetArtist(artistId);

        }

        public Task<dynamic> GetPlaylists(string username)
        {
            throw new NotImplementedException();
        }

        public Task<dynamic> GetPlaylists(string username, int offset)
        {
            throw new NotImplementedException();
        }

        public async Task<dynamic> GetRecommendation(string artistSeed)
        {
            throw new NotImplementedException();
        }

        public async Task<dynamic> GetRecommendation(string artistSeed, int limit)
        {
            throw new NotImplementedException();
        }

        public async Task<dynamic> GetRelatedArtists(string artistId)
        {
            return await api.GetRelatedArtists(artistId);
        }

        public Task PlayArtist(string userHash, string spotifyUri)
        {
            throw new NotImplementedException();
        }

        public async Task<dynamic> SearchArtists(string artistId)
        {
            return await api.SearchArtists(artistId);

        }

        public async Task<dynamic> SearchArtists(string artistId, int limit)
        {
            return await api.SearchArtists(artistId, limit);
        }

    }

}
