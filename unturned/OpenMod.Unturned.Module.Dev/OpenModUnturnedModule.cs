using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using OpenMod.API;
using OpenMod.NuGet;
using OpenMod.Unturned.Module.Shared;
using SDG.Unturned;

namespace OpenMod.Unturned.Module.Dev
{
    public class OpenModUnturnedModule : OpenModUnturnedModuleBase
    {
        protected override void Initialize()
        {
            string openModDirectory = Path.GetFullPath($"Servers/{Dedicator.serverID}/OpenMod/");
            if (!Directory.Exists(openModDirectory))
            {
                Directory.CreateDirectory(openModDirectory);
            }

            var parameters = new RuntimeInitParameters
            {
                CommandlineArgs = Environment.GetCommandLineArgs(),
                WorkingDirectory = openModDirectory
            };

            var assemblies = new List<Assembly>
            {
                typeof(OpenMod.UnityEngine.UnityMainThreadDispatcher).Assembly,
                typeof(OpenModUnturnedHost).Assembly
            };

            var runtime = new Runtime.Runtime();
            runtime.Init(assemblies, parameters);
        }
    }
}