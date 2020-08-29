using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using OpenMod.API;
using OpenMod.UnityEngine.Plugins;
using OpenMod.Unturned.Module.Shared;
using SDG.Framework.Modules;
using SDG.Unturned;

namespace OpenMod.Unturned.Module.Dev
{
    public class OpenModUnturnedModule : IModuleNexus
    {
        public Runtime.Runtime OpenModRuntime { get; private set; }
        
        private OpenModSharedUnturnedModule m_SharedModule;

        public void initialize()
        {
            m_SharedModule = new OpenModSharedUnturnedModule();
            if (!m_SharedModule.Initialize(GetType().Assembly))
                return;
            OnInitialize();
        }

        public void shutdown()
        {
            m_SharedModule.Shutdown();
            OpenModRuntime = null;
        }

        private void OnInitialize()
        {
            Environment.CurrentDirectory = ReadWrite.PATH;
            var openModDirectory = Path.Combine(ReadWrite.PATH, $"Servers/{Dedicator.serverID}/OpenMod/");
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
                typeof(OpenModUnityEnginePlugin).Assembly,
                typeof(OpenModUnturnedHost).Assembly
            };

            OpenModRuntime = new Runtime.Runtime();
            OpenModRuntime.Init(assemblies, parameters);
        }
    }
}