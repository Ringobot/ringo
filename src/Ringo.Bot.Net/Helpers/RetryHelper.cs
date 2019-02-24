using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RingoBotNet.Helpers
{
    public static class RetryHelper
    {
        /// <summary>
        /// Retries a function that results in an exception `times` times before throwing the exception.
        /// </summary>
        /// <typeparam name="T">The `T` in the async `Task of T`</typeparam>
        /// <param name="func">The function to try</param>
        /// <param name="times">Number of times to try the function before throwing an exception</param>
        /// <param name="waitMs">Amount of time to wait before retrying in milliseconds</param>
        /// <param name="backoff">When `true` will exponentially backoff the wait time between each try, e.g.
        /// if `waitMs = 1000` and `times = 3`, will wait 1000 ms before the second try and 2000 ms before the third try.</param>
        /// <param name="logger">Optional <see cref="ILogger"/> to log information and errors to.</param>
        /// <param name="cancellationToken">Optional <see cref="CancellationToken"/> to cancel the retry operations.</param>
        /// <returns>The result of the function as Task of T.</returns>
        public async static Task<T> RetryAsync<T>(
            Func<Task<T>> func,
            int times = 3,
            int waitMs = 1000,
            bool backoff = true,
            ILogger logger = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            //TODO: I can't figure out how to make these overloads more generic and DRY

            int i = 1;

            while (i <= times)
            {
                try
                {
                    return await func();
                }
                catch (Exception ex)
                {
                    LogOrThrow(ex, logger, i, func, waitMs, times, cancellationToken);
                }

                if (waitMs > 0) waitMs = await Wait(waitMs, backoff, cancellationToken);

                i++;
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Retries a function that results in an exception `times` times before throwing the exception.
        /// </summary>
        /// <param name="func">The function to try</param>
        /// <param name="times">Number of times to try the function before throwing an exception</param>
        /// <param name="waitMs">Amount of time to wait before retrying in milliseconds</param>
        /// <param name="backoff">When `true` will exponentially backoff the wait time between each try, e.g.
        /// if `waitMs = 1000` and `times = 3`, will wait 1000 ms before the second try and 2000 ms before the third try.</param>
        /// <param name="logger">Optional <see cref="ILogger"/> to log information and errors to.</param>
        /// <param name="cancellationToken">Optional <see cref="CancellationToken"/> to cancel the retry operations.</param>
        public async static Task RetryAsync(
            Func<Task> func,
            int times = 3,
            int waitMs = 1000,
            bool backoff = true,
            ILogger logger = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            int i = 1;

            while (i <= times)
            {
                try
                {
                    await func();
                    return;
                }
                catch (Exception ex)
                {
                    LogOrThrow(ex, logger, i, func, waitMs, times, cancellationToken);
                }

                if (waitMs > 0) waitMs = await Wait(waitMs, backoff, cancellationToken);
                i++;
            }

            throw new InvalidOperationException();
        }

        private static async Task<int> Wait(int waitMs, bool backoff, CancellationToken cancellationToken)
        {
            await Task.Delay(waitMs, cancellationToken);
            if (backoff) waitMs = waitMs * 2;
            if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();
            return waitMs;
        }

        private static void LogOrThrow(
            Exception ex, 
            ILogger logger, 
            int i, 
            object func, 
            int waitMs, 
            int times, 
            CancellationToken cancellationToken)
        {
            logger?.LogError(ex, ex.Message);

            if (i < times)
            {
                logger?.LogInformation($"Try {i} of {func} failed with {ex.Message}. Retrying in {waitMs} ms.");
                if (cancellationToken.IsCancellationRequested) throw ex;
            }
            else
            {
                logger?.LogInformation($"Final try {i} of {func} failed with {ex.Message}.");
                throw ex;
            }
        }
    }
}
