using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using OpenMod.Bootstrapper;
using OpenMod.NuGet;
using OpenMod.Unturned.Module.Shared;
using SDG.Framework.Modules;
using SDG.Unturned;

namespace OpenMod.Unturned.Module
{
    public class OpenModUnturnedModule : IModuleNexus
    {
        private OpenModSharedUnturnedModule m_SharedModule;

        public void initialize()
        {
            m_SharedModule = new OpenModSharedUnturnedModule();
            m_SharedModule.Initialize(GetType().Assembly);
            OnInitialize();
        }

        public void shutdown()
        {
            m_SharedModule.Shutdown();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void OnInitialize()
        {
            string openModDirectory = Path.GetFullPath($"Servers/{Dedicator.serverID}/OpenMod/");
            if (!Directory.Exists(openModDirectory))
            {
                Directory.CreateDirectory(openModDirectory);
            }

            Console.WriteLine("Bootstrapping OpenMod for Unturned, this might take a while...");

            var bootrapper = new OpenModDynamicBootstrapper();

            bootrapper.Bootstrap(
                openModDirectory,
                Environment.GetCommandLineArgs(),
                new List<string> { "OpenMod.Unturned" },
                false,
                new NuGetConsoleLogger());
        }
    }
}