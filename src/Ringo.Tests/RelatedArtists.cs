//using Microsoft.Extensions.Configuration;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Newtonsoft.Json;
//using Ringo.Common.Models;
//using Ringo.Common.Helpers;
//using Ringo.Functions;
//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Threading.Tasks;
//using Ringo.Functions.Artists;

//namespace Ringo.Tests
//{
//    [TestClass]
//    public class RelatedArtistTest
//    {
//        [TestCategory("Integration")]
//        [TestMethod]
//        public async Task Run_CreateRelationship_DoesNotError()
//        {
//            // initialise

//            // arrange
//            Entity testUser = TestHelper.NewEntity("testUser", "user");
//            Entity testArtist = TestHelper.NewEntity("incubus:373c591c05ed02146136d1ceb704191f", "Incubus", "artist");
//            EntityRelationship entityRelationship = new EntityRelationship();
//            entityRelationship.FromVertex = testUser;
//            entityRelationship.ToVertex = testArtist;
//            entityRelationship.Relationship = "likes";
//            entityRelationship.RelationshipDate = DateTime.UtcNow;

//            // act
//            try
//            {
//                var mockMsgString = JsonConvert.SerializeObject(entityRelationship);
//                //await RelatedArtists.Start(mockMsgString, null, null);


//                // assert
//            }
//            catch
//            {
//                throw;
//            }
//            finally
//            {
//                // teardown
//                var cleanupFrom = await GraphHelper.RemoveVertex(testUser.Id);
//                var cleanupTo = await GraphHelper.RemoveVertex(testArtist.Id);
//            }

//        }

//        [TestInitialize]
//        public void Init()
//        {
//            IConfiguration config = TestHelper.GetIConfigurationRoot();
//            Environment.SetEnvironmentVariable("CosmosGraphEndpoint", config.GetValue<string>("CosmosGraphEndpoint"));
//            Environment.SetEnvironmentVariable("CosmosGraphKey", config.GetValue<string>("CosmosGraphKey"));
//            Environment.SetEnvironmentVariable("CosmosGraphDB", config.GetValue<string>("CosmosGraphDB"));
//            Environment.SetEnvironmentVariable("CosmosGraphCollection", config.GetValue<string>("CosmosGraphCollection"));

//        }
//    }
//}
