using System;
using System.Net.Http;
using System.Threading.Tasks;
using SpotifyApi.NetCore;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Ringo.Common.Services
{

    public class SpotifyService
    {
        private static HttpClient httpClient = new HttpClient();
        private static ClientCredentialsAuthorizationApi auth = new ClientCredentialsAuthorizationApi(httpClient);
        private static ArtistsApi api = new ArtistsApi(httpClient, auth);

        public async Task<List<dynamic>> GetArtist(string artistId)
        {
            Console.WriteLine(artistId);
            dynamic response = await api.GetArtist(artistId);
            return response;

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
            return await FakeArtist(artistSeed);
        }

        public async Task<dynamic> GetRecommendation(string artistSeed, int limit)
        {
            return await FakeArtist(artistSeed, limit);
        }

        public async Task<dynamic> GetRelatedArtists(string artistId)
        {
            return await FakeArtist(artistId);
        }

        public Task PlayArtist(string userHash, string spotifyUri)
        {
            throw new NotImplementedException();
        }

        public async Task<dynamic> SearchArtists(string artist)
        {
            return await FakeArtist(artist);
        }

        public async Task<dynamic> SearchArtists(string artist, int limit)
        {
            return await FakeArtist(artist, limit);
        }

        private async Task<List<dynamic>> FakeArtist(string artist, int limit = 3)
        {
            List<dynamic> list = new List<dynamic>();
            if (artist.Length > 0)
            {
                dynamic result = new
                {
                    name = "Jackie Wilson",
                    spotify = new
                    {
                        id = "4VnomLtKTm9Ahe1tZfmZju",
                        uri = "spotify:artist:4VnomLtKTm9Ahe1tZfmZju"
                    },
                    image = new
                    {
                        height = 640,
                        width = 480,
                        url = "https://i.scdn.co/image/66cadd255a5f01b6496c0854bed1267888b312d1"
                    }
                };
                list.Add(result);
            }

            return list;
        }
    }

}
