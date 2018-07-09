using Microsoft.Extensions.Configuration;
using SpotifyApi.NetCore;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ringo.Common.Services
{

    public class SpotifyService : ISpotifyService
    {
        private static HttpClient _httpClient;
        private static IConfiguration _configuration;
        private static ClientCredentialsAuthorizationApi _auth;
        private static ArtistsApi _api;

        public SpotifyService(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
            _auth = new ClientCredentialsAuthorizationApi(_httpClient, configuration);
            _api = new ArtistsApi(_httpClient, _auth);
        }

        public SpotifyService() : this(null)
        {
        }

        public async Task<dynamic> GetArtist(string artistId)
        {
            return await _api.GetArtist(artistId);

        }

        public Task<dynamic> GetPlaylists(string username)
        {
            throw new NotImplementedException();
        }

        public Task<dynamic> GetPlaylists(string username, int offset)
        {
            throw new NotImplementedException();
        }

        public Task<dynamic> GetRecommendation(string artistSeed, int limit = 3)
        {
            throw new NotImplementedException();
        }

        public async Task<dynamic> GetRelatedArtists(string artistId)
        {
            return await _api.GetRelatedArtists(artistId);
        }

        public Task PlayArtist(string userHash, string spotifyUri)
        {
            throw new NotImplementedException();
        }

        public async Task<dynamic> SearchArtists(string artistId, int limit = 3)
        {
            return await _api.SearchArtists(artistId, limit);
        }

    }

}
