using Microsoft.VisualStudio.TestTools.UnitTesting;
using RingoBotNet.Helpers;
using System;
using System.Threading.Tasks;

namespace RingoBotNet.Tests.Helpers
{
    [TestClass]
    public class RetryHelperTests
    {
        [TestMethod]
        public async Task Retry_3TimesNoError_Invokes1Time()
        {
            // arrange
            int i = 0;

            // act
            await RetryHelper.Retry(() => i++, times: 3, waitMs: 0);

            // assert
            Assert.AreEqual(1, i);
        }

        private async Task<int> AddOne(int i) => await Task.Run(() => ++i);

        private async Task<int> AddOneError(int i) => await Task.Run(() => ++i / 0);

        [TestMethod]
        public async Task Retry_AsyncFunc_Invokes1Time()
        {
            // arrange
            int i = 0;

            // act
            int result = await RetryHelper.RetryAsync(() => AddOne(i), times: 3, waitMs: 0);

            // assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public async Task Retry_AsyncFuncError_ThrowsException()
        {
            // act
            await RetryHelper.RetryAsync(() => AddOneError(1), times: 3, waitMs: 0);
        }

        [TestMethod]
        public async Task Retry_3TimesError_Invokes3Times()
        {
            // arrange
            int i = 0;

            // act
            try
            {
                await RetryHelper.Retry(() => i++ / 0, times: 3, waitMs: 0);
            }
            catch
            {
            }

            // assert
            Assert.AreEqual(3, i);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public async Task Retry_3TimesError_ThrowsException()
        {
            // arrange
            int i = 0;

            // act
            await RetryHelper.Retry(() => i++ / 0, times: 3, waitMs: 0);
        }
    }
}
