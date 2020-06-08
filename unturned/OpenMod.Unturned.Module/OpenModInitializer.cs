using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using SDG.Unturned;

#if NUGET_BOOTSTRAP
using OpenMod.Bootstrapper;
using OpenMod.NuGet;
#else
using System.Reflection;
using Microsoft.Extensions.Hosting;
using OpenMod.API;
#endif


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
            Console.WriteLine("Bootstrapping OpenMod for Unturned, this might take a while...");

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