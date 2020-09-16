using System;
using System.Threading.Tasks;

namespace OpenMod.Core.Helpers
{
    public sealed class AsyncDisposeAction : IAsyncDisposable
    {
        private readonly Func<Task> m_DisposeTask;

        public AsyncDisposeAction(Func<Task> disposeTask)
        {
            m_DisposeTask = disposeTask;
        }

        public async ValueTask DisposeAsync()
        {
            if (m_DisposeTask == null)
            {
                return;
            }

            await m_DisposeTask.Invoke();
        }
    }
}