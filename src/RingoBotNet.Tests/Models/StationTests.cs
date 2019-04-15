using Microsoft.VisualStudio.TestTools.UnitTesting;
using RingoBotNet.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RingoBotNet.Tests.Models
{
    [TestClass]
    public class StationTests
    {
        [TestMethod]
        public void EncodeIds_ValidInput_IdPkRegExMatch()
        {
            // arrange
            var info = new ConversationInfo
            {
                ChannelId = "slack",
                ChannelTeamId = "abc123",
                ConversationName = "Music Lovers"
            };

            const string hashtag = "ElvisLives";

            // act
            (string id, string pk) = Station.EncodeIds(info, hashtag);

            // assert
            Assert.IsTrue(Station.StationIdRegex.IsMatch(id));
            Assert.IsTrue(Station.StationPKRegex.IsMatch(pk));
        }
    }

}
