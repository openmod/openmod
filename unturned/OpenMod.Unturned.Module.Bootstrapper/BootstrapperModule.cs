using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
//using OpenMod.Common.Hotloading;
using SDG.Framework.Modules;
using Path = System.IO.Path;

namespace OpenMod.Unturned.Module.Bootstrapper
{
    [UsedImplicitly]
    public class BootstrapperModule : IModuleNexus
    {
        private IModuleNexus m_BootstrappedModule;
        public static BootstrapperModule Instance { get; private set; }
        private string s_SelfLocation;

        public void initialize()
        {
            Instance = this;

            if (string.IsNullOrEmpty(s_SelfLocation))
            {
                s_SelfLocation = Path.GetFullPath(typeof(BootstrapperModule).Assembly.Location);
            }

            var openModModuleDir = Path.GetDirectoryName(s_SelfLocation);
            Assembly moduleAssembly = null;

            // ReSharper disable once AssignNullToNotNullAttribute
            foreach (var assemblyFile in Directory.GetFiles(openModModuleDir, "*.dll", SearchOption.TopDirectoryOnly))
            {
                if (Path.GetFullPath(assemblyFile) == s_SelfLocation)
                {
                    continue;
                }

                //Hotloader.Enabled = true;
                //var assembly = Hotloader.LoadAssembly(File.ReadAllBytes(assemblyFile));
                var assembly = Assembly.Load(File.ReadAllBytes(assemblyFile));
                var fileName = Path.GetFileName(assemblyFile);
                if ((fileName.Equals("OpenMod.Unturned.Module.dll") && moduleAssembly != null)
                    || fileName.Equals("OpenMod.Unturned.Module.Dev.dll"))
                {
                    moduleAssembly = assembly;
                }
            }

            if (moduleAssembly == null)
            {
                throw new Exception("Failed to find OpenMod module assembly!");
            }

            ICollection<Type> types;
            try
            {
                types = moduleAssembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(d => d != null).ToList();
            }

            var moduleType = types.SingleOrDefault(d => d.Name.Equals("OpenModUnturnedModule"));
            if (moduleType == null)
            {
                throw new Exception($"Failed to find OpenModUnturnedModule class in {moduleAssembly}!");
            }

            m_BootstrappedModule = (IModuleNexus)Activator.CreateInstance(moduleType);
            m_BootstrappedModule.initialize();
        }

        public void shutdown()
        {
            m_BootstrappedModule?.shutdown();
            Instance = null;
        }
    }
}
