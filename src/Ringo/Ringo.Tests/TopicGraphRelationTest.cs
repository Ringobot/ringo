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
            EntityRelationship entityRelationship = new EntityRelationship();
            entityRelationship.FromVertex = testUser;
            entityRelationship.ToVertex = testArtist;
            entityRelationship.Relationship = "likes";
            entityRelationship.RelationshipDate = DateTime.UtcNow;

            // act
            try
            {
                //string mockMsgString = $@"{ ""FromVertex"":{ ""Id"":""test-user{}"",""Name"":""default-user"",""Properties"":{""type"":""user""}},""ToVertex"":{""Id"":""test-artist"",""Name"":""Metallica"",""Properties"":{""type"":""artist""}},""Relationship"":""likes"",""RelationshipDate"":""2018-04-13T22:31:17.935Z""}";
                var mockMsgString = JsonConvert.SerializeObject(entityRelationship);
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
                var cleanupFrom = await GraphHelper.RemoveVertex(testUser.Id);
                var cleanupTo = await GraphHelper.RemoveVertex(testArtist.Id);
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