using System;

namespace OpenMod.Core.Helpers
{
    public sealed class DisposeAction : IDisposable
    {
        private readonly Action m_DisposeAction;

        public DisposeAction(Action disposeAction)
        {
            m_DisposeAction = disposeAction;
        }

        public void Dispose()
        {
            m_DisposeAction?.Invoke();
        }
    }
}