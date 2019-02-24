using Microsoft.VisualStudio.TestTools.UnitTesting;
using RingoBotNet.Helpers;
using System;
using System.Threading.Tasks;

namespace RingoBotNet.Tests.Helpers
{
    [TestClass]
    public class RetryHelperTests
    {
        private async Task<int> AddOne(int i) => await Task.Run(() => ++i);

        private async Task<int> AddOneError(int i) => await Task.Run(() => ++i / 0);

        private async Task DoNothing() => await Task.Delay(1);

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
        public async Task Retry_AsyncFuncReturnVoid_NoError()
        {
            // act
            await RetryHelper.RetryAsync(() => DoNothing(), times: 3, waitMs: 0);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public async Task Retry_AsyncFuncError_ThrowsException()
        {
            // act
            await RetryHelper.RetryAsync(() => AddOneError(1), times: 3, waitMs: 0);
        }
    }
}
