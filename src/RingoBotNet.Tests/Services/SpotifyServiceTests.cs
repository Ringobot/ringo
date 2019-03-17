using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RingoBotNet.Services;
using System.Threading.Tasks;
using SpotifyApi.NetCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace RingoBotNet.Tests.Services
{
    [TestClass]
    public class SpotifyServiceTests
    {
        [TestMethod]
        public async Task GetOffset_SameThreeRoundtripResults_ReturnsMin()
        {
            // arrange
            var mockLogger = new Mock<ILogger<RingoService>>();
            // ClassUnderTest
            var cut = new Mock<RingoService>(null, null, null, null, null, null, null, mockLogger.Object)
            { CallBase = true };
            cut.Setup(c => c.GetRoundTrip(It.IsAny<string>())).ReturnsAsync((1000, 1000));

            // act
            long? result = await cut.Object.GetOffset(string.Empty);

            // assert
            Assert.AreEqual(1500, result ?? 0);
        }

        [TestMethod]
        public async Task GetOffset_ThreeRoundtripResults_ReturnsMin()
        {
            // arrange
            var results = new Stack<(long, long)>(new (long, long)[] { (1000, 1000), (2000, 2000), (3000, 3000) });
            var mockLogger = new Mock<ILogger<RingoService>>();
            // ClassUnderTest
            var cut = new Mock<RingoService>(null, null, null, null, null, null, null, mockLogger.Object)
                { CallBase = true };
            cut.Setup(c => c.GetRoundTrip(It.IsAny<string>())).ReturnsAsync(results.Pop);

            // act
            long? result = await cut.Object.GetOffset(string.Empty);

            // assert
            Assert.AreEqual(1500, result ?? 0);
        }

    }
}
