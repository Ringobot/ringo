using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RingoBotNet.Data;
using RingoBotNet.Helpers;
using RingoBotNet.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RingoBotNet.Tests.Data
{
    [TestClass]
    public class UserDataTests
    {
        [TestMethod]
        public async Task SaveUserAccessToken_StateTokenSet_StateTokenStillSet()
        {
            // arrange
            const string state = "abc123";
            const string userId = "def456";
            var token = new BearerAccessToken();
            var user = new User { SpotifyAuth = new Auth { State = state } };

            var mockLogger = new Mock<ILogger<UserData>>();

            // mock IConfiguration
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.SetReturnsDefault("ghi789");
            mockConfiguration.Setup(c => c[ConfigHelper.CosmosDBEndpoint]).Returns("https://localtest.me/");
            mockConfiguration.Setup(c => c[ConfigHelper.CosmosDBPrimaryKey]).Returns("aGVsbG93b3JsZA==");
            
            // Use Moq to create a Class Under Test. Mock out the Read and Replace methods
            var dataCut = new Mock<UserData>(mockConfiguration.Object, mockLogger.Object) { CallBase = true };
            dataCut.Setup(d => d.Read<User>(It.IsAny<(string, string)>())).ReturnsAsync(user);
            dataCut.Setup(d => d.Replace(It.IsAny<CosmosEntity>())).Returns(Task.CompletedTask);

            // act
            await dataCut.Object.SaveUserAccessToken(userId, token);

            // assert
            Assert.AreEqual(state, user.SpotifyAuth.State);
        }
    }
}
