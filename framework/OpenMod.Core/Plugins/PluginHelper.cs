using System.IO;
using OpenMod.API;

namespace OpenMod.Core.Plugins
{
    public static class PluginHelper
    {
        public static string GetWorkingDirectory(IRuntime runtime, string pluginId)
        {
            return Path.Combine(runtime.WorkingDirectory, "plugins", pluginId);
        }
    }
}