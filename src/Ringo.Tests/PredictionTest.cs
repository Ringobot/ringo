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
    public class PredictionTest
    {
        [TestCategory("Unit")]
        [TestMethod]
        public async Task GetPrediction_DoesNotError()
        {
            // arrange
            string canonJson = File.ReadAllText(".\\TestData\\canon.json");
            RdostrId[] rdostrId = JsonConvert.DeserializeObject<RdostrId[]>(canonJson);
            string radiohead = "Radiohead";
            string jackieWilson = "Jackie Wilson";
            // act
            try
            {
                Artist predict = await PredictArtist.GetPrediction();

                Assert.AreEqual(radiohead, predict.name);
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
