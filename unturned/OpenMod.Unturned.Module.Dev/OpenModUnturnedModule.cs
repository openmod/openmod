using OpenMod.API;
using OpenMod.UnityEngine.Plugins;
using OpenMod.Unturned.Module.Shared;
using SDG.Framework.Modules;
using SDG.Unturned;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace OpenMod.Unturned.Module.Dev
{
    public class OpenModUnturnedModule : IModuleNexus
    {
        public Runtime.Runtime? OpenModRuntime { get; private set; }

        private OpenModSharedUnturnedModule? m_SharedModule;

        public void initialize()
        {
            m_SharedModule = new OpenModSharedUnturnedModule();

            if (!m_SharedModule.Initialize(isDynamicLoad: false))
            {
                return;
            }

            OnInitialize();
        }

        public void shutdown()
        {
            m_SharedModule?.Shutdown();
            OpenModRuntime = null;
        }

        private void OnInitialize()
        {
            System.Environment.CurrentDirectory = ReadWrite.PATH;
            var openModDirectory = Path.Combine(ReadWrite.PATH, $"Servers/{Dedicator.serverID}/OpenMod/");
            if (!Directory.Exists(openModDirectory))
            {
                Directory.CreateDirectory(openModDirectory);
            }

            var parameters = new RuntimeInitParameters
            {
                CommandlineArgs = System.Environment.GetCommandLineArgs(),
                WorkingDirectory = openModDirectory,
                PackageManager = m_SharedModule!.GetNugetPackageManager(openModDirectory)
            };

            var assemblies = new List<Assembly>
            {
                typeof(OpenModUnityEnginePlugin).Assembly,
                typeof(OpenModUnturnedHost).Assembly
            };

            OpenModRuntime = new Runtime.Runtime();
            OpenModRuntime.Init(assemblies, parameters);
            m_SharedModule.OnPostInitialize();
        }
    }
}