using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace OpenMod.Core.Helpers
{
    public static class ProcessHelper
    {
        public static Task WaitForExitAsync(this Process process,
            CancellationToken cancellationToken = default)
        {
            if (process.HasExited) return Task.CompletedTask;

            var tcs = new TaskCompletionSource<object?>();
            process.EnableRaisingEvents = true;
            process.Exited += (_, _) => tcs.TrySetResult(result: null);

            if (cancellationToken != default)
            {
                cancellationToken.Register(() => tcs.SetCanceled());
            }

            return process.HasExited ? Task.CompletedTask : tcs.Task;
        }
    }
}