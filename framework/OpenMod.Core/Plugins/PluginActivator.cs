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

namespace OpenMod.Core.Plugins
{
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class PluginActivator : IPluginActivator, IAsyncDisposable
    {
        private readonly IRuntime m_Runtime;
        private readonly ILogger<PluginActivator> m_Logger;
        private readonly IStringLocalizerFactory m_StringLocalizerFactory;
        private readonly ILifetimeScope m_LifetimeScope;
        private readonly IDataStoreFactory m_DataStoreFactory;
        private bool m_IsDisposing;

        public PluginActivator(
            IRuntime runtime,
            ILogger<PluginActivator> logger,
            IStringLocalizerFactory stringLocalizerFactory,
            ILifetimeScope lifetimeScope,
            IDataStoreFactory dataStoreFactory)
        {
            m_Runtime = runtime;
            m_Logger = logger;
            m_StringLocalizerFactory = stringLocalizerFactory;
            m_LifetimeScope = lifetimeScope;
            m_DataStoreFactory = dataStoreFactory;
        }

        private readonly List<WeakReference> m_ActivatedPlugins = new List<WeakReference>();
        public IReadOnlyCollection<IOpenModPlugin> ActivatedPlugins
        {
            get
            {
                if (m_IsDisposing)
                {
                    throw new ObjectDisposedException(nameof(PluginActivator));
                }

                return m_ActivatedPlugins.Where(d => d.IsAlive).Select(d => d.Target).Cast<IOpenModPlugin>().ToList();
            }
        }

        [CanBeNull]
        public async Task<IOpenModPlugin> TryActivatePluginAsync(Assembly assembly)
        {
            if (m_IsDisposing)
            {
                throw new ObjectDisposedException(nameof(PluginActivator));
            }

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
                var serviceProvider = m_LifetimeScope.Resolve<IServiceProvider>();
                var lifetimeScope = m_LifetimeScope.BeginLifetimeScope(containerBuilder =>
                {
                    var workingDirectory = PluginHelper.GetWorkingDirectory(m_Runtime, pluginMetadata.Id);

                    var configurationBuilder = new ConfigurationBuilder();
                    if (Directory.Exists(workingDirectory))
                    {
                        configurationBuilder
                            .SetBasePath(workingDirectory)
                            .AddYamlFile("config.yaml", optional: true, reloadOnChange: true);
                    }

                    var configuration = configurationBuilder
                        .AddEnvironmentVariables(pluginMetadata.Id.Replace(".", "_") + "_")
                        .Build();

                    containerBuilder.Register(context => configuration)
                        .As<IConfiguration>()
                        .As<IConfigurationRoot>()
                        .SingleInstance()
                        .OwnedByLifetimeScope();

                    containerBuilder.RegisterType(pluginType)
                        .As(pluginType)
                        .As<IOpenModPlugin>()
                        .SingleInstance()
                        .ExternallyOwned();

                    containerBuilder.Register(context => m_DataStoreFactory.CreateDataStore(null, workingDirectory))
                        .As<IDataStore>()
                        .SingleInstance()
                        .OwnedByLifetimeScope();

                    var stringLocalizer = m_StringLocalizerFactory.Create("translations", workingDirectory);
                    containerBuilder.Register(context => stringLocalizer)
                        .As<IStringLocalizer>()
                        .SingleInstance()
                        .OwnedByLifetimeScope();

                    foreach (var type in pluginType.Assembly.FindTypes<IPluginContainerConfigurator>())
                    {
                        var configurator = (IPluginContainerConfigurator) ActivatorUtilities.CreateInstance(serviceProvider, type);
                        configurator.ConfigureContainer(containerBuilder);
                    }
                });

                pluginInstance = (IOpenModPlugin)lifetimeScope.Resolve(pluginType);
            }
            catch (Exception ex)
            {
                m_Logger.LogError(ex, $"Failed to load plugin from type: {pluginType.FullName} in assembly: {assembly.FullName}");
                return null;
            }

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
            if (m_IsDisposing)
            {
                return;
            }

            m_IsDisposing = true;
            m_Logger.LogInformation("Unloading all plugins...");

            int i = 0;
            foreach (var plugin in m_ActivatedPlugins)
            {
                if (plugin.IsAlive)
                {
                    var instance = (IOpenModPlugin)plugin.Target;

                    try
                    {
                        await instance.UnloadAsync();
                        i++;
                    }
                    catch (Exception ex)
                    {
                        m_Logger.LogError(ex, $"An exception occured while unloading {instance.DisplayName}");
                    }
                }
            }

            m_ActivatedPlugins.Clear();
            m_Logger.LogInformation($"> {i} plugins unloaded.");
        }
    }
}