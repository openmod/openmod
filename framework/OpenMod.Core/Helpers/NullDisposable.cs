using System;

namespace OpenMod.Core.Helpers
{
    public class NullDisposable : IDisposable
    {
        private static NullDisposable? s_Instance;
        public static NullDisposable Instance
        {
            get
            {
                return s_Instance ??= new NullDisposable();
            }
        }

        private NullDisposable()
        {
            
        }
        public void Dispose()
        {
        }
    }
}