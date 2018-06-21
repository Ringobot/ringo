using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Ringo.Common.Models;
using Ringo.Functions;
using System;
using System.Threading.Tasks;

namespace Ringo.Tests
{
    [TestClass]
    public class TopicDBTest
    {
        [TestCategory("Integration")]
        [TestMethod]
        public async Task Run_CreateDocs_DoesNotError()
        {
            // initialise

            // arrange
            //Entity testUser = TestHelper.NewEntity("testUser", "user");
            Entity testArtist1 = TestHelper.NewEntity("incubus:373c591c05ed02146136d1ceb704191f:1", "Incubus", "artist");
            Entity testArtist2 = TestHelper.NewEntity("incubus:373c591c05ed02146136d1ceb704191f:2", "Incubus", "artist");
            EntityRelationship entityRelationship = new EntityRelationship();
            entityRelationship.FromVertex = testArtist1;
            entityRelationship.ToVertex = testArtist2;
            entityRelationship.Relationship = "likes";
            entityRelationship.RelationshipDate = DateTime.UtcNow;

            // act
            try
            {
                var mockMsgString = JsonConvert.SerializeObject(entityRelationship);
                await TopicCosmosCreate.Run(mockMsgString, null);


                // assert
            }
            catch
            {
                throw;
            }
            finally
            {
                // teardown
                var cleanupFrom = await CosmosHelper.RemoveVertex(testArtist1.Id);
                var cleanupTo = await CosmosHelper.RemoveVertex(testArtist2.Id);
            }

        }

        [TestInitialize]
        public void Init()
        {
            IConfiguration config = TestHelper.GetIConfigurationRoot();
            Environment.SetEnvironmentVariable("CosmosEndpoint", config.GetValue<string>("CosmosEndpoint"));
            Environment.SetEnvironmentVariable("CosmosKey", config.GetValue<string>("CosmosKey"));
            Environment.SetEnvironmentVariable("CosmosDB", config.GetValue<string>("CosmosDB"));
            Environment.SetEnvironmentVariable("CosmosCollection", config.GetValue<string>("CosmosCollection"));

        }
    }
}
