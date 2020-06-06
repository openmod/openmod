using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NuGet.Common;
using OpenMod.API;
using OpenMod.NuGet;
using OpenMod.Runtime;
using SDG.Unturned;

namespace OpenMod.Unturned.Module
{
    public class OpenModInitializer
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Initialize()
        {
            string openModDirectory = Path.GetFullPath($"Servers/{Dedicator.serverID}/OpenMod/");
            if (!Directory.Exists(openModDirectory))
            {
                Directory.CreateDirectory(openModDirectory);
            }

#if NUGET_BOOTSTRAP
            UnturnedLog.info("Bootstrapping OpenMod for Unturned, this might take a while...");

            var bootrapper = new OpenModDynamicBootstrapper();

            bootrapper.Bootstrap(
                openModDirectory,
                Environment.GetCommandLineArgs(),
                new List<string> { "OpenMod.Unturned" },
                false,
                new NuGetConsoleLogger());

#else
            var hostBuilder = new HostBuilder();
            var parameters = new RuntimeInitParameters
            {
                CommandlineArgs = Environment.GetCommandLineArgs(),
                WorkingDirectory = openModDirectory
            };

            var assemblies = new List<Assembly>
            {
                typeof(OpenMod.UnityEngine.UnityMainThreadDispatcher).Assembly,
                typeof(OpenMod.Unturned.OpenModUnturnedHost).Assembly
            };

            var runtime = new Runtime.Runtime();
            runtime.Init(assemblies, hostBuilder, parameters);
#endif
        }
    }
}