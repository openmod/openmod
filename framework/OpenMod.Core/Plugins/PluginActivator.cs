using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Yaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Persistence;
using OpenMod.API.Plugins;
using OpenMod.API.Prioritization;
using OpenMod.Core.Helpers;
using OpenMod.Core.Localization;
using OpenMod.Core.Persistence;

namespace OpenMod.Core.Plugins
{
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class PluginActivator : IPluginActivator, IAsyncDisposable
    {
        private readonly IRuntime m_Runtime;
        private readonly ILogger<PluginActivator> m_Logger;
        private readonly ILifetimeScope m_LifetimeScope;
        private readonly IDataStoreFactory m_DataStoreFactory;

        public PluginActivator(
            IRuntime runtime,
            ILogger<PluginActivator> logger,
            ILifetimeScope lifetimeScope,
            IDataStoreFactory dataStoreFactory)
        {
            m_Runtime = runtime;
            m_Logger = logger;
            m_LifetimeScope = lifetimeScope;
            m_DataStoreFactory = dataStoreFactory;
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
                    var workingDirectory = PluginHelper.GetWorkingDirectory(m_Runtime, pluginMetadata.Id);

                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(workingDirectory)
                        .AddYamlFile("config.yml", optional: true)
                        .AddEnvironmentVariables(pluginMetadata.Id.Replace(".", "_") + "_")
                        .Build();

                    containerBuilder.Register(context => configuration)
                        .As<IConfigurationRoot>()
                        .As<IConfiguration>()
                        .SingleInstance()
                        .OwnedByLifetimeScope();

                    containerBuilder.RegisterType(pluginType)
                        .As(pluginType)
                        .SingleInstance();

                    containerBuilder.Register(context => m_DataStoreFactory.CreateDataStore(workingDirectory))
                        .As<IDataStore>()
                        .SingleInstance()
                        .OwnedByLifetimeScope();

                    if (File.Exists(Path.Combine(workingDirectory, "translations.yml")))
                    {
                        var translations = new ConfigurationBuilder()
                            .SetBasePath(workingDirectory)
                            .AddYamlFile("translations.yml")
                            .Build();

                        var stringLocalizer = new ConfigurationStringLocalizer(translations);
                        containerBuilder.Register(context => stringLocalizer)
                            .As<IStringLocalizer>()
                            .SingleInstance()
                            .OwnedByLifetimeScope();
                    }
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

        public async ValueTask DisposeAsync()
        {
            m_Logger.LogInformation("Unloading all plugins...");

            foreach (var plugin in m_ActivatedPlugins)
            {
                if (plugin.IsAlive)
                {
                    var instance = (IOpenModPlugin) plugin.Target;
                    await instance.UnloadAsync();
                }
            }

            m_ActivatedPlugins.Clear();
            m_Logger.LogInformation("Plugins unloaded.");
        }
    }
}