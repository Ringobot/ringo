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
            var mockLogger = new Mock<ILogger<RingoService>>();
            // ClassUnderTest
            var cut = new Mock<RingoService>(null, null, null, null, null, null, null, mockLogger.Object)
            { CallBase = true };
            cut.Setup(c => c.GetRoundTrip(It.IsAny<string>())).ReturnsAsync((1000, 1000, DateTime.UtcNow));

            // act
            (bool success, long progressMs, DateTime utc) = await cut.Object.GetOffset(string.Empty);

            // assert
            Assert.AreEqual(1500, progressMs);
        }

        [TestMethod]
        public async Task GetOffset_ThreeRoundtripResults_ReturnsMin()
        {
            // arrange
            var results = new Stack<(long, long, DateTime)>(new (long, long, DateTime)[] 
                {
                    (1000, 1000, DateTime.UtcNow),
                    (2000, 2000, DateTime.UtcNow),
                    (3000, 3000, DateTime.UtcNow)
                });

            var mockLogger = new Mock<ILogger<RingoService>>();
            // ClassUnderTest
            var cut = new Mock<RingoService>(null, null, null, null, null, null, null, mockLogger.Object)
                { CallBase = true };
            cut.Setup(c => c.GetRoundTrip(It.IsAny<string>())).ReturnsAsync(results.Pop);

            // act
            (bool success, long progressMs, DateTime utc) = await cut.Object.GetOffset(string.Empty);

            // assert
            Assert.AreEqual(1500, progressMs);
        }
    }
}
