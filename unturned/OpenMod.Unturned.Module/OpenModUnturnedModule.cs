extern alias JetBrainsAnnotations;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using JetBrainsAnnotations::JetBrains.Annotations;
using OpenMod.Bootstrapper;
using OpenMod.NuGet;
using OpenMod.Unturned.Module.Shared;
using SDG.Framework.Modules;
using SDG.Unturned;

namespace OpenMod.Unturned.Module
{
    [UsedImplicitly]
    public class OpenModUnturnedModule : IModuleNexus
    {
        public object? OpenModRuntime { get; private set; }
        public bool IsDynamicLoad { get; set; }

        private OpenModSharedUnturnedModule? m_SharedModule;

        public void initialize()
        {
            m_SharedModule = new OpenModSharedUnturnedModule();
            if (!m_SharedModule.Initialize(GetType().Assembly, IsDynamicLoad))
            {
                return;
            }

            OnInitialize();
        }

        public void shutdown()
        {
            m_SharedModule?.Shutdown();
            m_SharedModule = null;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void OnInitialize()
        {
            Environment.CurrentDirectory = ReadWrite.PATH;
            var openModDirectory = Path.Combine(ReadWrite.PATH, $"Servers/{Dedicator.serverID}/OpenMod/");
            if (!Directory.Exists(openModDirectory))
            {
                Directory.CreateDirectory(openModDirectory);
            }

            Console.WriteLine("Bootstrapping OpenMod for Unturned, this might take a while...");

            var bootrapper = new OpenModDynamicBootstrapper();

            OpenModRuntime = bootrapper.Bootstrap(
                m_SharedModule!.GetNugetPackageManager(openModDirectory),
                openModDirectory,
                Environment.GetCommandLineArgs(),
                new[] { "OpenMod.UnityEngine", "OpenMod.Unturned" },
                allowPrereleaseVersions: false,
                new NuGetConsoleLogger());

            m_SharedModule.OnPostInitialize();
        }
    }
}