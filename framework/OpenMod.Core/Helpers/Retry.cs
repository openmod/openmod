using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenMod.Core.Helpers
{
    public static class Retry
    {
        public static async Task DoAsync(Action action, TimeSpan retryInterval, int maxAttempts)
        {
            var exceptions = new List<Exception>();

            for (var attempted = 0; attempted < maxAttempts; attempted++)
            {
                try
                {
                    if (attempted > 0)
                    {
                        await Task.Delay(retryInterval);
                    }

                    action();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            throw new AggregateException(exceptions);
        }

        public static async Task<T> DoAsync<T>(Func<Task<T>> action, TimeSpan retryInterval, int maxAttempts)
        {
            var exceptions = new List<Exception>();

            for (var attempted = 0; attempted < maxAttempts; attempted++)
            {
                try
                {
                    if (attempted > 0)
                    {
                        await Task.Delay(retryInterval);
                    }

                    return await action();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            throw new AggregateException(exceptions);
        }

        public static async Task<T> DoAsync<T>(Func<T> action, TimeSpan retryInterval, int maxAttempts)
        {
            var exceptions = new List<Exception>();

            for (var attempted = 0; attempted < maxAttempts; attempted++)
            {
                try
                {
                    if (attempted > 0)
                    {
                        await Task.Delay(retryInterval);
                    }

                    return action();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            throw new AggregateException(exceptions);
        }
    }
}
