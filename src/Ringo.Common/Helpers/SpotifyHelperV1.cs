using Ringo.Common.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;


namespace Ringo.Common.Heplers
{

    public class SpotifyHelperv1
     {
        private static string tokenUrl = "https://accounts.spotify.com/api/token";
        private static string authUrl = "https://accounts.spotify.com/authorize";
        private static HttpClient client = new HttpClient();
        private static string SpotifyApiToken = Environment.GetEnvironmentVariable("SpotifyApiToken");

        public async static Task<Artist> GetArtist(string artistId)
        {
            return FakeArtist();
        }

        public async static Task<List<Artist>> GetRelatedArtists(string artistId)
        {
            Artist artist = FakeArtist();
            List<Artist> artists = new List<Artist>();
            artists.Add(artist);
            return artists;

        }

        public async static Task<Artist> GetArtistByUri(string artistId)
        {
            return FakeArtist();
        }

        public static dynamic FakeArtist()
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
            return result;
        }
    }

}
