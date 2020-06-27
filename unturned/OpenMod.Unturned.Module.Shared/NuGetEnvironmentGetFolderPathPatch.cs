using System;
using NuGet.Common;

namespace OpenMod.Unturned.Module.Shared
{
    // This patch uses some magic values because of this private enum:
    // https://github.com/NuGet/NuGet.Client/blob/636570e/src/NuGet.Core/NuGet.Common/PathUtil/NuGetEnvironment.cs#L338
    public static class NuGetEnvironmentGetFolderPathPatch
    {
        public static bool GetFolderPath(int folder /* private SpecialFolder enum */, ref string __result)
        {
            if (folder == 0x3 /* SpecialFolder.CommonApplicationData */ && !RuntimeEnvironmentHelper.IsWindows)
            {
                if (RuntimeEnvironmentHelper.IsMacOSX)
                {
                    __result = @"/Library/Application Support";
                }
                else
                {
                    var commonApplicationDataOverride = Environment.GetEnvironmentVariable("NUGET_COMMON_APPLICATION_DATA");

                    __result = !string.IsNullOrEmpty(commonApplicationDataOverride) 
                        ? commonApplicationDataOverride 
                        : @"/etc/opt";
                }

                return false;
            }

            return true;
        }
    }
}