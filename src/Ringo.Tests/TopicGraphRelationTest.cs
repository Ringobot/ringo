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
    public class TopicGraphRelationCreateTest
    {
        [TestCategory("Integration")]
        [TestMethod]
        public async Task Run_SomeData_DoesNotError()
        {
            // initialise

            // arrange
            var fromVertex = Guid.NewGuid().ToString("N");
            var toVertex = Guid.NewGuid().ToString("N");
            string relationship = "testRelationship"; 
            // act
            try
            {
                await GraphHelper.CreateVertex(fromVertex);
                await GraphHelper.CreateVertex(toVertex);
                GremlinRelationship mockMsg = new GremlinRelationship() { FromVertex = fromVertex, Relationship = relationship, ToVertex = toVertex };
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
                HttpStatusCode cleanupFrom = await GraphHelper.RemoveVertex(fromVertex);
                HttpStatusCode cleanupTo = await GraphHelper.RemoveVertex(toVertex);
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
