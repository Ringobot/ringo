using Microsoft.VisualStudio.TestTools.UnitTesting;
using RingoBotNet.Helpers;

namespace RingoBotNet.Tests
{
    [TestClass]
    public class CryptoHelperTests
    {
        [TestMethod]
        public void Sha256_Expected_Actual()
        {
            // arrange
            const string input = "38e08093d1b642e3aef9c5e22cc5f0c0";
            const string expected = "EC66CAC7E8C5859FE2483024FE02A6A7153FFB186986E748A91A454295E32E73";

            // act
            string actual = CryptoHelper.Sha256(input);

            // assert
            Assert.AreEqual(expected, actual);
        }
    }
}
