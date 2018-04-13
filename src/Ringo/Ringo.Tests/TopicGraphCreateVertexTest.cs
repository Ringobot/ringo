using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Ringo.Common.Models;
using Ringo.Functions;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Ringo.Tests
{
    [TestClass]
    public class TopicGraphCreateVertexTest
    {
        [TestCategory("Integration")]
        [TestMethod]
        public async Task Run_CreateVertex_DoesNotError()
        {
            // initialise

            // arrange
            Entity testUser = TestHelper.NewEntity("testUser");

            // act
            try
            {
                var mockMsgString = JsonConvert.SerializeObject(testUser);
                await TopicGraphCreateEntity.Run(mockMsgString, null);


                // assert
            }
            catch
            {
                throw;
            }
            finally
            {
                // teardown
                HttpStatusCode cleanupFrom = await GraphHelper.RemoveVertex(testUser.id);
            }

        }

        [TestInitialize]
        public void Init()
        {

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .AddEnvironmentVariables()
                .Build();

            Environment.SetEnvironmentVariable("CosmosGraphEndpoint", config.GetValue<string>("CosmosGraphEndpoint"));
            Environment.SetEnvironmentVariable("CosmosGraphKey", config.GetValue<string>("CosmosGraphKey"));
            Environment.SetEnvironmentVariable("CosmosGraphDB", config.GetValue<string>("CosmosGraphDB"));
            Environment.SetEnvironmentVariable("CosmosGraphCollection", config.GetValue<string>("CosmosGraphCollection"));


        }
    }
}
