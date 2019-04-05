using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RingoBotNet.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;

namespace RingoBotNet.Tests.Services
{
    [TestClass]
    public class SpotifyServiceTests
    {
        [TestMethod]
        public async Task GetOffset_SameThreeRoundtripResults_ReturnsMin()
        {
            // arrange
            var mockLogger = new Mock<ILogger<SpotifyService>>();
            // ClassUnderTest
            var cut = new Mock<SpotifyService>(null, null, null, null, null, null, null, mockLogger.Object)
            { CallBase = true };
            cut.Setup(c => c.GetRoundTrip(It.IsAny<string>())).ReturnsAsync((null, 1000, 1000, DateTime.UtcNow));

            // act
            (bool success, string itemId, (long positionMs, DateTime utc) position) result = await cut.Object.GetOffset(string.Empty);

            // assert
            Assert.AreEqual(1500, result.position.positionMs);
        }

        [TestMethod]
        public async Task GetOffset_ThreeRoundtripResults_ReturnsMin()
        {
            // arrange
            var results = new Stack<(string, long, long, DateTime)>(new (string, long, long, DateTime)[] 
                {
                    (null, 1000, 1000, DateTime.UtcNow),
                    (null, 2000, 2000, DateTime.UtcNow),
                    (null, 3000, 3000, DateTime.UtcNow)
                });

            var mockLogger = new Mock<ILogger<SpotifyService>>();
            // ClassUnderTest
            var cut = new Mock<SpotifyService>(null, null, null, null, null, null, null, mockLogger.Object)
                { CallBase = true };
            cut.Setup(c => c.GetRoundTrip(It.IsAny<string>())).ReturnsAsync(results.Pop);

            // act
            (bool success, string itemId, (long positionMs, DateTime utc) position) result = await cut.Object.GetOffset(string.Empty);

            // assert
            Assert.AreEqual(1500, result.position.positionMs);
        }
    }
}
