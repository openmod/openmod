using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Yaml;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Plugins;
using OpenMod.Core.Helpers;

namespace OpenMod.Core.Plugins
{
    [UsedImplicitly]
    [ServiceImplementation]
    public class PluginActivator : IPluginActivator
    {
        private readonly IRuntime m_Runtime;
        private readonly ILogger<PluginActivator> m_Logger;
        private readonly ILifetimeScope m_LifetimeScope;

        public PluginActivator(
            IRuntime runtime,
            ILogger<PluginActivator> logger,
            ILifetimeScope lifetimeScope)
        {
            m_Runtime = runtime;
            m_Logger = logger;
            m_LifetimeScope = lifetimeScope;
        }

        private readonly List<WeakReference> m_ActivatedPlugins = new List<WeakReference>();
        public IReadOnlyCollection<IOpenModPlugin> ActivatedPlugins
        {
            get { return m_ActivatedPlugins.Where(d => d.IsAlive).Select(d => d.Target).Cast<IOpenModPlugin>().ToList(); }
        }

        [CanBeNull]
        public async Task<IOpenModPlugin> TryActivatePluginAsync(Assembly assembly)
        {
            var pluginMetadata = assembly.GetCustomAttribute<PluginMetadataAttribute>();
            if (pluginMetadata == null)
            {
                m_Logger.LogError($"Failed to load plugin from assembly {assembly}: couldn't find any plugin metadata");
                return null;
            }
            
            var pluginTypes = assembly.FindTypes<IOpenModPlugin>(false).ToList();
            if (pluginTypes.Count == 0)
            {
                m_Logger.LogError($"Failed to load plugin from assembly {assembly}: couldn't find any IOpenModPlugin implementation");
                return null;
            }

            if (pluginTypes.Count > 1)
            {
                m_Logger.LogError($"Failed to load plugin from assembly {assembly}: assembly has multiple IOpenModPlugin instances");
                return null;
            }

            var pluginType = pluginTypes.Single();
            IOpenModPlugin pluginInstance;
            try
            {
                var lifetimeScope = m_LifetimeScope.BeginLifetimeScope(containerBuilder =>
                {
                    var confiugration = new ConfigurationBuilder()
                        .SetBasePath(PluginHelper.GetWorkingDirectory(m_Runtime, pluginMetadata.Id))
                        .AddYamlFile("config.yml", optional: true)
                        .AddEnvironmentVariables(pluginMetadata.Id.Replace(".", "_") + "_")
                        .Build();

                    containerBuilder.Register(context => confiugration)
                        .As<IConfigurationRoot>()
                        .As<IConfiguration>()
                        .SingleInstance();

                    containerBuilder.RegisterType(pluginType)
                        .As(pluginType)
                        .SingleInstance();
                });

                pluginInstance = (IOpenModPlugin) lifetimeScope.Resolve(pluginType);
            }
            catch (Exception ex)
            {
                m_Logger.LogError(ex, $"Failed to load plugin from type: {pluginType.FullName} in assembly: {assembly.FullName}");
                return null;
            }

            m_Logger.LogInformation($"Loading plugin: {pluginInstance.DisplayName} v{pluginInstance.Version}");

            try
            {
                await pluginInstance.LoadAsync();
            }
            catch (Exception ex)
            {
                m_Logger.LogError(ex, $"Failed to load plugin: {pluginInstance.DisplayName} v{pluginInstance.Version}");
                return null;
            }

            m_ActivatedPlugins.Add(new WeakReference(pluginInstance));
            return pluginInstance;
        }
    }
}