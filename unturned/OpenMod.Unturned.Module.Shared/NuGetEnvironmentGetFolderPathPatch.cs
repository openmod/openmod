using NuGet.Common;

namespace OpenMod.Unturned.Module.Shared
{
    // Fix for https://github.com/openmod/openmod/issues/12#issuecomment-646818601
    public static class NuGetEnvironmentGetFolderPathPatch
    {
        public static bool GetFolderPath(NuGetFolderPath folder, ref string __result)
        {
            if (folder != NuGetFolderPath.MachineWideSettingsBaseDirectory || RuntimeEnvironmentHelper.IsWindows)
            {
                return true;
            }

            if (RuntimeEnvironmentHelper.IsMacOSX)
            {
                __result = @"/Library/Application Support/NuGet";
                return false;
            }
            
            if (RuntimeEnvironmentHelper.IsLinux)
            {
                __result = @"/etc/opt/NuGet";
                return false;
            }

            // unknown OS, let NuGet handle it
            return true;
        }
    }
}