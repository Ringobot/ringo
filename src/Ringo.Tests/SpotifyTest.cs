using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Ringo.Common.Models;
using Ringo.Common.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Ringo.Tests
{
    [TestClass]
    public class SpotifyTest
    {
        [TestCategory("Unit")]
        [TestMethod]
        public async Task GetArtist_DoesNotError()
        {
            // arrange
            SpotifyService spotifyService = new SpotifyService();

            // act
            try
            {
                dynamic artist = await spotifyService.GetArtist("1tpXaFf2F55E7kVJON4j4G");

                //Assert.AreEqual("", artist.name);
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
            Environment.SetEnvironmentVariable("SpotifyApiClientId", config.GetValue<string>("SpotifyApiClientId"));
            Environment.SetEnvironmentVariable("SpotifyApiClientSecret", config.GetValue<string>("SpotifyApiClientSecret"));
        }
    }
}
