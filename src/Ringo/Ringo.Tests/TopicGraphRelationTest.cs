using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Ringo.Common.Models;
using Ringo.Functions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Ringo.Tests
{
    [TestClass]
    public class TopicGraphRelationCreateTest
    {
        [TestCategory("Integration")]
        [TestMethod]
        public async Task Run_CreateRelationship_DoesNotError()
        {
            // initialise

            // arrange
            Entity testUser = TestHelper.NewEntity("testUser");
            Entity testArtist = TestHelper.NewEntity("testArtist");

            // act
            try
            {
                await GraphHelper.CreateVertex(testUser);
                await GraphHelper.CreateVertex(testArtist);
                GremlinRelationship mockMsg = new GremlinRelationship() { FromVertex = testUser.id, Relationship = "testRelationship", ToVertex = testArtist.id };
                var mockMsgString = JsonConvert.SerializeObject(mockMsg);
                await TopicGraphCreateRelationship.Run(mockMsgString, null);


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
                HttpStatusCode cleanupTo = await GraphHelper.RemoveVertex(testArtist.id);
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

        }
    }
}
