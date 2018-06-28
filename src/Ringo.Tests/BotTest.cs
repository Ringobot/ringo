//using Microsoft.Extensions.Configuration;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Newtonsoft.Json;
//using Ringo.Common.Models;
//using Ringo.Functions;
//using System;
//using System.Threading.Tasks;

//namespace Ringo.Tests
//{
//    [TestClass]
//    public class BotTest
//    {
//        [TestCategory("Integration")]
//        [TestMethod]
//        public async Task Run_SpiderBot_DoesNotError()
//        {
//            // arrange

//            // act
//            try
//            {
//                SpiderBot.Run(null, null);


//                // assert
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//                throw;
//            }

//        }

//        [TestInitialize]
//        public void Init()
//        {
//            IConfiguration config = TestHelper.GetIConfigurationRoot();
//            Environment.SetEnvironmentVariable("RingoDirectLine", config.GetValue<string>("RingoDirectLine"));
//            Environment.SetEnvironmentVariable("RingoBotId", config.GetValue<string>("RingoBotId"));
//            Environment.SetEnvironmentVariable("CosmosGraphEndpoint", config.GetValue<string>("CosmosGraphEndpoint"));
//            Environment.SetEnvironmentVariable("CosmosGraphKey", config.GetValue<string>("CosmosGraphKey"));
//            Environment.SetEnvironmentVariable("CosmosGraphDB", config.GetValue<string>("CosmosGraphDB"));
//            Environment.SetEnvironmentVariable("CosmosGraphCollection", config.GetValue<string>("CosmosGraphCollection"));

//        }
//    }
//}
