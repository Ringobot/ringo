using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RingoBotNet.Helpers
{
    public static class RetryHelper
    {
        public async static Task<T> Retry<T>(
            Func<T> func, 
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
                    return func();
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, ex.Message);

                    if (i < times)
                    {
                        logger?.LogInformation($"Try {i} of {func} failed with {ex.Message}. Retrying in {waitMs} ms.");
                    }
                    else
                    {
                        throw ex;
                    }
                }

                if (waitMs > 0)
                {
                    await Task.Delay(waitMs, cancellationToken);
                    waitMs = waitMs * 2;
                }

                i++;
            }

            throw new InvalidOperationException();
        }

        public async static Task<T> RetryAsync<T>(
            Func<Task<T>> func,
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
                    return await func();
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, ex.Message);

                    if (i < times)
                    {
                        logger?.LogInformation($"Try {i} of {func} failed with {ex.Message}. Retrying in {waitMs} ms.");
                    }
                    else
                    {
                        throw ex;
                    }
                }

                if (waitMs > 0)
                {
                    await Task.Delay(waitMs, cancellationToken);
                    waitMs = waitMs * 2;
                }

                i++;
            }

            throw new InvalidOperationException();
        }

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
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, ex.Message);

                    if (i < times)
                    {
                        logger?.LogInformation($"Try {i} of {func} failed with {ex.Message}. Retrying in {waitMs} ms.");
                    }
                    else
                    {
                        throw ex;
                    }
                }

                if (waitMs > 0)
                {
                    await Task.Delay(waitMs, cancellationToken);
                    waitMs = waitMs * 2;
                }

                i++;
            }

            throw new InvalidOperationException();
        }
    }
}
