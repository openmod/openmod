using System;

namespace OpenMod.Core.Helpers
{
    public static class PlatformHelper
    {
        public static bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }
    }
}