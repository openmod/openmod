using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using OpenMod.API.Plugins;
using OpenMod.Unturned.Plugins;
using SDG.Framework.IO;
using SDG.Framework.Modules;
using SDG.Unturned;
using Serilog;
using UnityEngine;
using Module = SDG.Framework.Modules.Module;

[assembly: PluginMetadata("Rocket.Unturned", Author = "OpenMod", DisplayName = "OpenMod RocketMod Plugin")]

namespace Rocket.Unturned
{
    public class RocketUnturnedOpenModPlugin : OpenModUnturnedPlugin
    {
        private readonly ILoggerFactory m_LoggerFactory;
        private U m_RocketComponent;

        private static readonly MethodInfo s_AreModuleDependenciesEnabled;
        private static readonly MethodInfo s_IsModuleDisabledByCommandLine;

        static RocketUnturnedOpenModPlugin()
        {
            s_AreModuleDependenciesEnabled = typeof(ModuleHook).GetMethod("areModuleDependenciesEnabled", BindingFlags.Static | BindingFlags.NonPublic);
            s_IsModuleDisabledByCommandLine = typeof(ModuleHook).GetMethod("isModuleDisabledByCommandLine", BindingFlags.Static | BindingFlags.NonPublic);
        }

        public RocketUnturnedOpenModPlugin(IServiceProvider serviceProvider, ILoggerFactory loggerFactory) : base(serviceProvider)
        {
            m_LoggerFactory = loggerFactory;
        }

        protected override async UniTask OnLoadAsync()
        {
            await base.OnLoadAsync();
            await UniTask.SwitchToMainThread();

            U.Logger = m_LoggerFactory.CreateLogger("RocketMod");
            U.PluginInstance = this;

            var rocketModDependents = FindRocketModModules()
                .Select(d => new Module(d))
                .ToList();

            Logger.LogInformation("RocketModDeps: " + rocketModDependents.Count);
            ModuleHook.modules.AddRange(rocketModDependents);

            foreach (var module in rocketModDependents)
            {
                var index = ModuleHook.modules.IndexOf(module);
                var shouldEnable = module.config.IsEnabled
                                   && (bool)s_AreModuleDependenciesEnabled.Invoke(null, new object[] { index })
                                   && (bool)s_IsModuleDisabledByCommandLine.Invoke(null, new object[] { module.config.Name });
                if (!shouldEnable)
                {
                    continue;
                }

                Logger.LogInformation($"Loading RocketMod module: {module.config.Name} v{module.config.Version}");
                module.isEnabled = true;
            }

            m_RocketComponent = new U();
            m_RocketComponent.initialize();
        }

        private List<ModuleConfig> FindRocketModModules()
        {
            string path = new DirectoryInfo(Application.dataPath).Parent.ToString();
            path = Path.Combine(path, "Modules");

            var modules = FindModules(path);
            modules.RemoveAll(d => ModuleHook.modules.Any(e => string.Equals(e.config.Name, d.Name, StringComparison.OrdinalIgnoreCase)));
            return modules
                .Where(d => d.Dependencies.Any(e => e.Name.Equals("Rocket.Unturned", StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        private List<ModuleConfig> FindModules(string path)
        {
            var configs = new List<ModuleConfig>();

            foreach (string moduleFile in Directory.GetFiles(path, "*.module"))
            {
                ModuleConfig moduleConfig = IOUtility.jsonDeserializer.deserialize<ModuleConfig>(moduleFile);
                if (moduleConfig != null)
                {
                    moduleConfig.DirectoryPath = path;
                    moduleConfig.FilePath = moduleFile;
                    moduleConfig.Version_Internal = Parser.getUInt32FromIP(moduleConfig.Version);
                    for (var i = moduleConfig.Dependencies.Count - 1; i >= 0; i--)
                    {
                        ModuleDependency moduleDependency = moduleConfig.Dependencies[i];
                        if (moduleDependency.Name == "Framework" || moduleDependency.Name == "Unturned")
                        {
                            moduleConfig.Dependencies.RemoveAtFast(i);
                        }
                        else
                        {
                            moduleDependency.Version_Internal = Parser.getUInt32FromIP(moduleDependency.Version);
                        }
                    }
                    configs.Add(moduleConfig);
                }
            }

            foreach (string subDirectory in Directory.GetDirectories(path))
            {
                configs.AddRange(FindModules(subDirectory));
            }

            return configs;
        }

        protected override async UniTask OnUnloadAsync()
        {
            await base.OnUnloadAsync();
            await UniTask.SwitchToMainThread();
            
            U.Logger = NullLogger.Instance;
            U.Instance?.shutdown();
            U.PluginInstance = null;

            if (m_RocketComponent != null) // can not use ? null conditional operator on Unity components
            {
                m_RocketComponent.shutdown();
            }
        }
    }
}
