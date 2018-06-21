using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ringo.Common.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Ringo.Functions
{

    //I'm going to leave this here
    //Started on this without thinking it through, this is basically reimplimenting the logic in the bot
    //Suggest creating a LUIS bypass so we can call the DirectLine without spamming the LUIS endpoint and exceeding quota
    class SpotifyHelper
    {
        private static string tokenUrl = "https://accounts.spotify.com/api/token";
        private static string authUrl = "https://accounts.spotify.com/authorize";
        private static HttpClient client = new HttpClient();
        private static string SpotifyApiToken = Environment.GetEnvironmentVariable("SpotifyApiToken");

        public static async Task BuildArtistGraph()
        {

            dynamic artlistList = await GraphHelper.GetArtistRelatedLessThanTwo();
            string accessToken = await GetToken();
            List<EntityRelationship> entityRelationships = await GetRecommendations(accessToken, artlistList);

        }

        public static async Task<string> GetToken()
        {
            try
            {
                Dictionary<string, string> grant = new Dictionary<string, string>();
                grant.Add("grant_type", "client_credentials");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", SpotifyApiToken);
                var requestToken = await client.PostAsync(tokenUrl, new FormUrlEncodedContent(grant));
                var tokenResult = await requestToken.Content.ReadAsStringAsync();
                var accessToken = JObject.Parse(tokenResult);
                return accessToken["access_token"].ToString();

            }
            catch (Exception)
            {

                throw;
            }

        }


        public static async Task<List<EntityRelationship>> GetRecommendations(string accessToken, dynamic artlistList)
        {
            List<EntityRelationship> entityRelationships = new List<EntityRelationship>();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            foreach (var spotifyId in artlistList)
            {
                var relatedArtistRequest = await client.GetAsync($"https://api.spotify.com/v1/artists/{spotifyId}/related-artists");
                SpotifyArtist spotifyArtist = JsonConvert.DeserializeObject<SpotifyArtist>(await relatedArtistRequest.Content.ReadAsStringAsync());
                Entity sourceArtist = new Entity(spotifyId, spotifyId, null);
                foreach (Artist a in spotifyArtist.artists)
                {
                    EntityRelationship relationship = new EntityRelationship();
                    Entity relatedArtist = new Entity(a.name, a.name, null);
                    relationship.FromVertex = sourceArtist;
                    relationship.Relationship = "related";
                    relationship.ToVertex = relatedArtist;
                    entityRelationships.Add(relationship);
                }
            }

            return entityRelationships;

        }

    }

}
