using Microsoft.VisualStudio.TestTools.UnitTesting;
using RingoBotNet.Models;

namespace RingoBotNet.Tests.Models
{
    [TestClass]
    public class UserTests
    {
        [TestMethod]
        public void EncodeId_UpperCaseUsername_EncodedSameAsLowerCaseUsername()
        {
            // arrange
            const string upper = "Daniel";
            const string lower = "daniel";

            var info = new ConversationInfo
            {
                ChannelId = "msteams",
                ChannelTeamId = "abc123"
            };

            var expected = User.EncodeIds(info, lower);

            // act
            var actual = User.EncodeIds(info, upper);

            // assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EncodeId_UpperCaseChannelId_EncodedSameAsLowerCaseChannelId()
        {
            // arrange
            const string userId = "daniel";

            var lower = new ConversationInfo
            {
                ChannelId = "msteams",
                ChannelTeamId = "abc123"
            };

            var upper = new ConversationInfo
            {
                ChannelId = "MSTEAMS",
                ChannelTeamId = "abc123"
            };

            var expected = User.EncodeIds(lower, userId);

            // act
            var actual = User.EncodeIds(upper, userId);

            // assert
            Assert.AreEqual(expected, actual);
        }
    }
}
