using Ringo.Common.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;
//using Feather.SpotifyApiDotNetCore;


namespace Ringo.Common.Heplers
{

    public class SpotifyHelper
    {
        private static string tokenUrl = "https://accounts.spotify.com/api/token";
        private static string authUrl = "https://accounts.spotify.com/authorize";
        private static HttpClient client = new HttpClient();
        private static string SpotifyApiToken = Environment.GetEnvironmentVariable("SpotifyApiToken");

        public async static Task<Artist> GetArtist(string artistId)
        {

            Artist artist = new Artist()
            {
                name = "Jackie Wilson",
                spotify = new Spotify()
                {
                    id = "4VnomLtKTm9Ahe1tZfmZju",
                    uri = "spotify:artist:4VnomLtKTm9Ahe1tZfmZju"
                },
                image = (new Image[]
                {
                    new Image()
                    {
                    height = 640,
                    width = 480,
                    url = "https://i.scdn.co/image/66cadd255a5f01b6496c0854bed1267888b312d1"
                    }
                })
            };
            return artist;
        }

    }

}
