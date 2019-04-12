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

            string expected = User.EncodeIds(channelId, lower).id;

            // act
            string actual = User.EncodeIds(channelId, upper).id;

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

            string expected = User.EncodeIds(lower, userId).id;

            // act
            string actual = User.EncodeIds(upper, userId).id;

            // assert
            Assert.AreEqual(expected, actual);
        }
    }
}
