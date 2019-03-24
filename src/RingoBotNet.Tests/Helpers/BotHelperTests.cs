using Microsoft.VisualStudio.TestTools.UnitTesting;
using RingoBotNet.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace RingoBotNet.Tests.Helpers
{
    [TestClass]
    public class BotHelperTests
    {
        [TestMethod]
        public void TokenForLogging_Null_ReturnsNull()
        {
            // act
            string actual = BotHelper.TokenForLogging(null);

            // assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TokenForLogging_EmptyString_ReturnsEmptyString()
        {
            // act
            string actual = BotHelper.TokenForLogging(string.Empty);

            // assert
            Assert.AreEqual(string.Empty, actual);
        }

        [TestMethod]
        public void TokenForLogging_3CharInput_ReturnsInput()
        {
            // arrange
            const string input = "123";
            
            // act
            string actual = BotHelper.TokenForLogging(input);

            // assert
            Assert.AreEqual(input, actual);
        }

        [TestMethod]
        public void TokenForLogging_5CharInput_ReturnsInput()
        {
            // arrange
            const string input = "12345";
            
            // act
            string actual = BotHelper.TokenForLogging(input);

            // assert
            Assert.AreEqual(input, actual);
        }

        [TestMethod]
        public void TokenForLogging_6CharInput_ReturnsFirst5CharsPlusDotDotDot()
        {
            // arrange
            const string input = "123456";
            
            // act
            string actual = BotHelper.TokenForLogging(input);

            // assert
            Assert.AreEqual("12345...", actual);
        }

        [TestMethod]
        public void TokenForLogging_TokenInput_ReturnsFirst5CharsOfTokenPlusDotDotDot()
        {
            // arrange
            const string input = "abc123def456ghi789jkl012abc123def456ghi789jkl012abc123def456ghi789jkl012";
            const string expected = "abc12...";
            
            // act
            string actual = BotHelper.TokenForLogging(input);

            // assert
            Assert.AreEqual(expected, actual);
        }

    }
}
