using Microsoft.VisualStudio.TestTools.UnitTesting;
using RingoBotNet.Models;
using System;
using System.Collections.Generic;
using System.Text;

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
            const string channelId = "msteams";

            string expected = User.EncodeId(channelId, lower);

            // act
            string actual = User.EncodeId(channelId, upper);

            // assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EncodeId_UpperCaseChannelId_EncodedSameAsLowerCaseChannelId()
        {
            // arrange
            const string upper = "MSTEAMS";
            const string lower = "msteams";
            const string userId = "daniel";

            string expected = User.EncodeId(lower, userId);

            // act
            string actual = User.EncodeId(upper, userId);

            // assert
            Assert.AreEqual(expected, actual);
        }
    }
}
