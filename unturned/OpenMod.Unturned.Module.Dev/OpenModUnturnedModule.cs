using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using OpenMod.API;
using OpenMod.NuGet;
using OpenMod.Unturned.Module.Shared;
using SDG.Framework.Modules;
using SDG.Unturned;

namespace OpenMod.Unturned.Module.Dev
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
        
        private void OnInitialize()
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