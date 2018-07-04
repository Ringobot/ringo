using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Ringo.Common.Helpers;
using Ringo.Common.Models;
using Ringo.Common.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ringo.Tests
{
    [TestClass]
    public class ArtistTest
    {
        ArtistService artistService = new ArtistService();

        [TestCategory("Unit")]
        [TestMethod]
        public void MapDataToArtist_DoesNotError()
        {
            // arrange
            string spotifyJson = File.ReadAllText(".\\TestData\\artistSpotify.json");
            dynamic spotifyList = JsonConvert.DeserializeObject<dynamic>(spotifyJson);
            // act
            try
            {
                Artist artist = ArtistHelper.MapToArtist(spotifyList);

                // assert
                Assert.AreEqual("Band of Horses", artist.name);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        [TestCategory("Unit")]
        [TestMethod]
        public async Task PushRelatedArtist_DoesNotError()
        {
            // arrange
          

            // act
            try
            {
                var baseArtist = await artistService.GetArtist("4VnomLtKTm9Ahe1tZfmZju");
                var relatedArtist = await artistService.GetRelatedArtists(baseArtist.spotify.id);
                List<EntityRelationship> entityRelationships = artistService.PushRelatedArtist(baseArtist, relatedArtist);

                // assert 
                Assert.AreEqual(baseArtist.name, entityRelationships[0].FromVertex.Name);
                Assert.AreEqual(relatedArtist[0].name, entityRelationships[0].ToVertex.Name);
                Assert.AreEqual("related", entityRelationships[0].Relationship);
                Assert.AreEqual(relatedArtist.Count, entityRelationships.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        [TestCategory("Unit")]
        [TestMethod]
        public async Task GetArtist_DoesNotError()
        {
            var result = await artistService.GetArtist("4VnomLtKTm9Ahe1tZfmZju");

            Assert.AreEqual("Jackie Wilson", result.name);
            Assert.AreEqual("spotify:artist:4VnomLtKTm9Ahe1tZfmZju", result.spotify.uri);
        }

        [TestCategory("Unit")]
        [TestMethod]
        public async Task GetArtistByUri_DoesNotError()
        {
            var result = await artistService.GetArtistByUri("spotify:artist:4VnomLtKTm9Ahe1tZfmZju");

            Assert.AreEqual("spotify:artist:4VnomLtKTm9Ahe1tZfmZju", result.spotify.uri);
        }

        [TestCategory("Unit")]
        [TestMethod]
        public async Task GetRelatedArtists_DoesNotError()
        {
            var result = await artistService.GetRelatedArtists("4VnomLtKTm9Ahe1tZfmZju");

            Assert.IsTrue(result.Count > 0);
        }

        [TestCategory("Unit")]
        [TestMethod]
        public async Task SearchArtists_DoesNotError()
        {
            var result = await artistService.SearchArtists("Jackie Wilson", 3);

            Assert.AreEqual("spotify:artist:4VnomLtKTm9Ahe1tZfmZju", result[0].spotify.uri);
            Assert.AreEqual(3, result.Count);
        }


        [TestInitialize]
        public void Init()
        {
            IConfiguration config = TestHelper.GetIConfigurationRoot();
            Environment.SetEnvironmentVariable("SpotifyApiClientId", config.GetValue<string>("SpotifyApiClientId"));
            Environment.SetEnvironmentVariable("SpotifyApiClientSecret", config.GetValue<string>("SpotifyApiClientSecret"));

        }
    }
}
