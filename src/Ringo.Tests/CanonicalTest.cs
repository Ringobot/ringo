using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Ringo.Common.Models;
using Ringo.Common.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ringo.Tests
{
    [TestClass]
    public class CanonicalTest
    {
        [TestCategory("Unit")]
        [TestMethod]
        public void GetCanonical_DoesNotError()
        {
            // arrange
            string canonJson = File.ReadAllText(".\\TestData\\canon.json");
            RdostrId[] rdostrId = JsonConvert.DeserializeObject<RdostrId[]>(canonJson);
            string radiohead = "Radiohead";
            string jackieWilson = "Jackie Wilson";
            CanonicalService canonicalService = new CanonicalService();
            // act
            try
            {

                RdostrId artistRdostrId0 = canonicalService.GetArtistId(radiohead);
                RdostrId artistRdostrId1 = canonicalService.GetArtistId(jackieWilson);

                // assert
                Assert.AreEqual(rdostrId[0].Urn, artistRdostrId0.Urn);
                Assert.AreEqual(rdostrId[0].UrnId, artistRdostrId0.UrnId);
                Assert.AreEqual(rdostrId[1].Urn, artistRdostrId1.Urn);
                Assert.AreEqual(rdostrId[1].UrnId, artistRdostrId1.UrnId);
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
