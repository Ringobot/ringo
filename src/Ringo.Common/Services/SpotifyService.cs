using Ringo.Common.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using SpotifyApiDotNetCore;
using System.Collections.Generic;

namespace Ringo.Common.Services
{

    public class SpotifyService
    {
        public async Task<List<dynamic>> GetArtist(string artistId)
        {
            List<dynamic> list = new List<dynamic>();
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
            return list;

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
