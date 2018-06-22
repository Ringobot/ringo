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
            string artistJson = File.ReadAllText(".\\TestData\\artist.json");
            string radiohead = "Radiohead";
            ArtistService artistService = new ArtistService();
            // act
            try
            {

                Artists artist = artistService.MapToArtist(artistJson);

                // assert
                Assert.AreEqual(radiohead, artist.artists[0].name);
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

                Artists artists = artistService.MapToArtist(artistsJson);
                List<EntityRelationship> entityRelationships = artistService.PushRelatedArtist(artists.artists[0], artists);

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


        [TestInitialize]
        public void Init()
        {
            IConfiguration config = TestHelper.GetIConfigurationRoot();
        }
    }
}
