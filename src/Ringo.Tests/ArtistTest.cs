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
        ArtistService artistService = new ArtistService();
        static string artistsJson = File.ReadAllText(".\\TestData\\artists.json");
        static string radiohead = "Radiohead";
        List<dynamic> artistsList = JsonConvert.DeserializeObject<List<dynamic>>(artistsJson);

        [TestCategory("Unit")]
        [TestMethod]
        public void MapDataToArtist_DoesNotError()
        {
            // arrange

            // act
            try
            {
                List<Artist> artist = artistService.MapToArtist(artistsList);

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
          

            // act
            try
            {
                
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
            var result = await artistService.GetArtist("4VnomLtKTm9Ahe1tZfmZju");

            Assert.AreEqual("Jackie Wilson", result[0].name);
            Assert.AreEqual("spotify:artist:4VnomLtKTm9Ahe1tZfmZju", result[0].spotify.uri);
        }

        [TestCategory("Unit")]
        [TestMethod]
        public async Task GetArtistByUri_DoesNotError()
        {
            var result = await artistService.GetArtistByUri("spotify:artist:4VnomLtKTm9Ahe1tZfmZju");

            Assert.AreEqual("spotify:artist:4VnomLtKTm9Ahe1tZfmZju", result[0].spotify.uri);
        }

        [TestCategory("Unit")]
        [TestMethod]
        public async Task GetRelatedArtists_DoesNotError()
        {
            var result = await artistService.GetRelatedArtists("spotify:artist:4VnomLtKTm9Ahe1tZfmZju");

            Assert.AreEqual("spotify:artist:4VnomLtKTm9Ahe1tZfmZju", result[0].spotify.uri);
        }

        [TestCategory("Unit")]
        [TestMethod]
        public async Task SearchArtists_DoesNotError()
        {
            var result = await artistService.SearchArtists("spotify:artist:4VnomLtKTm9Ahe1tZfmZju");

            Assert.AreEqual("spotify:artist:4VnomLtKTm9Ahe1tZfmZju", result[0].spotify.uri);
        }

        [TestCategory("Unit")]
        [TestMethod]
        public async Task FindArtistMatch_DoesNotError()
        {
            (bool result, List<Artist> artists) = await artistService.FindArtistMatch("spotify:artist:4VnomLtKTm9Ahe1tZfmZju");

            if (result)
            {
                Assert.AreEqual("spotify:artist:4VnomLtKTm9Ahe1tZfmZju", artists[0].spotify.uri);
            }

        }

        [TestCategory("Unit")]
        [TestMethod]
        public async Task FindArtistNoMatch_DoesNotError()
        {
            (bool result, List<Artist> artists) = await artistService.FindArtistMatch(string.Empty);

            if (result)
            {
                Assert.AreEqual(0, artists.Count);
            }

        }


        [TestInitialize]
        public void Init()
        {
            IConfiguration config = TestHelper.GetIConfigurationRoot();
        }
    }
}
