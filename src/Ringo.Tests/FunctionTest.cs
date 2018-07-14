using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Ringo.Common.Helpers;
using Ringo.Functions;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ringo.Tests
{
    [TestClass]
    public class FunctionTest
    {
        [TestCategory("Integration")]
        [TestMethod]
        public async Task Run_GetArtist_DoesNotError()
        {
            var req = new DefaultHttpContext().Request;
            var requestFeature = req.HttpContext.Features.Get<IHttpRequestFeature>();
            requestFeature.Method = "get";
            requestFeature.QueryString = "artist=4VnomLtKTm9Ahe1tZfmZju";
            string type = "id";

            // act
            try
            {
                var request = await GetArtist.Run(req, type, null);
                var okObjectResult = request as OkObjectResult;
                Assert.IsNotNull(okObjectResult);
                // assert
            }
            catch
            {
                throw;
            }

        }

        [TestCategory("Integration")]
        [TestMethod]
        public async Task Run_GetArtistByUri_DoesNotError()
        {
            var req = new DefaultHttpContext().Request;
            var requestFeature = req.HttpContext.Features.Get<IHttpRequestFeature>();
            requestFeature.Method = "get";
            requestFeature.QueryString = "artist=spotify:artist:4VnomLtKTm9Ahe1tZfmZju";
            string type = "uri";

            // act
            try
            {
                var request = await GetArtist.Run(req, type, null);
                var okObjectResult = request as OkObjectResult;
                Assert.IsNotNull(okObjectResult);
                // assert
            }
            catch
            {
                throw;
            }
            finally
            {
                // teardown

            }

        }

        [TestCategory("Integration")]
        [TestMethod]
        public async Task Run_GetRelatedArtists_DoesNotError()
        {
            var req = new DefaultHttpContext().Request;
            var requestFeature = req.HttpContext.Features.Get<IHttpRequestFeature>();
            requestFeature.Method = "get";
            requestFeature.QueryString = "artist=4VnomLtKTm9Ahe1tZfmZju";
            string type = "related";

            // act
            try
            {
                var request = await GetArtist.Run(req, type, null);
                var okObjectResult = request as OkObjectResult;
                Assert.IsNotNull(okObjectResult);
                // assert
            }
            catch
            {
                throw;
            }

        }

        [TestCategory("Integration")]
        [TestMethod]
        public async Task Run_SearchArtists_DoesNotError()
        {
            var req = new DefaultHttpContext().Request;
            var requestFeature = req.HttpContext.Features.Get<IHttpRequestFeature>();
            requestFeature.Method = "get";
            requestFeature.QueryString = "artist=Jackie%20Wilson";
            string type = "search";
            // act
            try
            {
                var request = await GetArtist.Run(req, type, null);
                var okObjectResult = request as OkObjectResult;
                Assert.IsNotNull(okObjectResult);
                // assert
            }
            catch
            {
                throw;
            }

        }


        [TestInitialize]
        public void Init()
        {
            IConfiguration config = TestHelper.GetIConfigurationRoot();
            Environment.SetEnvironmentVariable("CosmosGraphEndpoint", config.GetValue<string>("CosmosGraphEndpoint"));
            Environment.SetEnvironmentVariable("CosmosGraphKey", config.GetValue<string>("CosmosGraphKey"));
            Environment.SetEnvironmentVariable("CosmosGraphDB", config.GetValue<string>("CosmosGraphDB"));
            Environment.SetEnvironmentVariable("CosmosGraphCollection", config.GetValue<string>("CosmosGraphCollection"));
            Environment.SetEnvironmentVariable("SpotifyApiClientId", config.GetValue<string>("SpotifyApiClientId"));
            Environment.SetEnvironmentVariable("SpotifyApiClientSecret", config.GetValue<string>("SpotifyApiClientSecret"));

        }
    }
}
