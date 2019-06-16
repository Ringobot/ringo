using Microsoft.VisualStudio.TestTools.UnitTesting;
using RingoBotNet.Models;

namespace RingoBotNet.Tests.Models
{
    [TestClass]
    public class UserTests
    {
        [TestMethod]
        public void EncodeId_UpperCaseChannelId_EncodedSameAsLowerCaseChannelId()
        {
            // arrange
            const string userId = "27:abc1234XYZ";

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

        [TestMethod]
        public void EncodeId_UserIdsWithDifferentCases_SameSame()
        {
            // arrange
            const string userId1 = "27:abc1234XYZ";
            const string userId2 = "27:ABC1234xyz";

            var info = new ConversationInfo
            {
                ChannelId = "msteams",
                ChannelTeamId = "abc123"
            };

            var encoded1 = User.EncodeIds(info, userId1);

            // act
            var encoded2 = User.EncodeIds(info, userId2);

            // assert
            Assert.AreNotEqual(encoded1, encoded2, "User Ids provided by channels are case sensitive");
        }
    }
}
