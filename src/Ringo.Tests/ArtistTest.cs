using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
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
        [TestCategory("Unit")]
        [TestMethod]
        public void MapDataToArtist_DoesNotError()
        {
            // arrange
            string artistJson = File.ReadAllText(".\\TestData\\artists.json");
            string radiohead = "Radiohead";
            ArtistService artistService = new ArtistService();
            // act
            try
            {
                List<dynamic> artists = JsonConvert.DeserializeObject<List<dynamic>>(artistJson);
                List<Artist> artist = artistService.MapToArtist(artists);

                // assert
                Assert.AreEqual(radiohead, artist[0].name);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        [TestCategory("Unit")]
        [TestMethod]
        public void PushRelatedArtist_DoesNotError()
        {
            // arrange
            string artistsJson = File.ReadAllText(".\\TestData\\artists.json");
            string radiohead = "Radiohead";
            ArtistService artistService = new ArtistService();

            // act
            try
            {
                List<dynamic> artistsList = JsonConvert.DeserializeObject<List<dynamic>>(artistsJson);
                List<Artist> artists = artistService.MapToArtist(artistsList);
                List<EntityRelationship> entityRelationships = artistService.PushRelatedArtist(artists[0], artists);

                // assert 
                Assert.AreEqual(radiohead, entityRelationships[0].FromVertex.Name);
                Assert.AreEqual(radiohead, entityRelationships[0].ToVertex.Name);
                Assert.AreEqual("related", entityRelationships[0].Relationship);
                Assert.AreEqual(3, entityRelationships.Count());
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
            ArtistService artistService = new ArtistService();

            var result = await artistService.GetArtist("spotify:artist:4VnomLtKTm9Ahe1tZfmZju");

            Assert.AreEqual("Jackie Wilson", result[0].name);
            //Assert.AreEqual("spotify:artist:4VnomLtKTm9Ahe1tZfmZju", result.spotify.uri);
        }

        [TestCategory("Unit")]
        [TestMethod]
        public async Task GetArtistByUri_DoesNotError()
        {
            ArtistService artistService = new ArtistService();

            var result = await artistService.GetArtistByUriAsync("spotify:artist:4VnomLtKTm9Ahe1tZfmZju");

            Assert.AreEqual("spotify:artist:4VnomLtKTm9Ahe1tZfmZju", result[0].spotify.uri);
        }

        [TestCategory("Unit")]
        [TestMethod]
        public async Task GetRelatedArtistsAsync_DoesNotError()
        {
            ArtistService artistService = new ArtistService();

            var result = await artistService.GetRelatedArtistsAsync("spotify:artist:4VnomLtKTm9Ahe1tZfmZju");

            Assert.AreEqual("spotify:artist:4VnomLtKTm9Ahe1tZfmZju", result[0].spotify.uri);
        }


        [TestInitialize]
        public void Init()
        {
            IConfiguration config = TestHelper.GetIConfigurationRoot();
        }
    }
}
