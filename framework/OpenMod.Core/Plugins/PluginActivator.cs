using Autofac;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Ioc;
using OpenMod.API.Permissions;
using OpenMod.API.Persistence;
using OpenMod.API.Plugins;
using OpenMod.API.Prioritization;
using OpenMod.Common.Helpers;
using OpenMod.Core.Helpers;
using OpenMod.Core.Ioc;
using OpenMod.Core.Ioc.Extensions;
using OpenMod.Core.Localization;
using OpenMod.Core.Permissions;
using OpenMod.Core.Plugins.Events;
using OpenMod.Core.Prioritization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace OpenMod.Core.Plugins
{
    [OpenModInternal]
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class PluginActivator : IPluginActivator, IAsyncDisposable
    {
        private readonly IRuntime m_Runtime;
        private readonly ILogger<PluginActivator> m_Logger;
        private readonly IStringLocalizerFactory m_StringLocalizerFactory;
        private readonly ILifetimeScope m_LifetimeScope;
        private readonly IDataStoreFactory m_DataStoreFactory;
        private readonly List<WeakReference> m_ActivatedPlugins;
        private readonly IEventBus m_EventBus;

        private bool m_IsDisposing;

        public PluginActivator(
            IRuntime runtime,
            ILogger<PluginActivator> logger,
            IStringLocalizerFactory stringLocalizerFactory,
            ILifetimeScope lifetimeScope,
            IDataStoreFactory dataStoreFactory, IEventBus eventBus)
        {
            m_Runtime = runtime;
            m_Logger = logger;
            m_StringLocalizerFactory = stringLocalizerFactory;
            m_LifetimeScope = lifetimeScope;
            m_DataStoreFactory = dataStoreFactory;
            m_EventBus = eventBus;
            m_ActivatedPlugins = new List<WeakReference>();
        }

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

        public async Task<IOpenModPlugin?> TryActivatePluginAsync(Assembly assembly)
        {
            try
            {
                if (m_IsDisposing)
                {
                    throw new ObjectDisposedException(nameof(PluginActivator));
                }

                var pluginMetadata = assembly.GetCustomAttribute<PluginMetadataAttribute>();
                if (pluginMetadata == null)
                {
                    m_Logger.LogError(
                        "Failed to load plugin from assembly {Assembly}: couldn't find any plugin metadata",
                        assembly);
                    return null;
                }

                var pluginTypes = assembly.FindTypes<IOpenModPlugin>().ToList();
                if (pluginTypes.Count == 0)
                {
                    m_Logger.LogError(
                        "Failed to load plugin from assembly {Assembly}: couldn't find any IOpenModPlugin implementation",
                        assembly);
                    return null;
                }

                if (pluginTypes.Count > 1)
                {
                    m_Logger.LogError(
                        "Failed to load plugin from assembly {Assembly}: assembly has multiple IOpenModPlugin instances",
                        assembly);
                    return null;
                }

                var pluginType = pluginTypes.Single();
                IOpenModPlugin pluginInstance;
                try
                {
                    var lifetimeScope = m_LifetimeScope.BeginLifetimeScopeEx(containerBuilder =>
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

                        containerBuilder.Register(_ => configuration)
                            .As<IConfiguration>()
                            .As<IConfigurationRoot>()
                            .SingleInstance()
                            .OwnedByLifetimeScope();

                        containerBuilder.RegisterType(pluginType)
                            .AsSelf()
                            .As<IOpenModComponent>()
                            .As<IOpenModPlugin>()
                            .SingleInstance()
                            .OwnedByLifetimeScope();

                        containerBuilder.RegisterType<ScopedPermissionChecker>()
                            .As<IPermissionChecker>()
                            .InstancePerLifetimeScope()
                            .OwnedByLifetimeScope();

                        containerBuilder.Register(_ => m_DataStoreFactory.CreateDataStore(new DataStoreCreationParameters
                        {
#pragma warning disable 618
                            ComponentId = pluginMetadata.Id,
#pragma warning restore 618
                            Prefix = null,
                            Suffix = "data",
                            WorkingDirectory = workingDirectory
                        }))
                            .As<IDataStore>()
                            .SingleInstance()
                            .OwnedByLifetimeScope();

                        var stringLocalizer = Directory.Exists(workingDirectory)
                            ? m_StringLocalizerFactory.Create("translations", workingDirectory)
                            : NullStringLocalizer.Instance;

                        containerBuilder.Register(_ => stringLocalizer)
                            .As<IStringLocalizer>()
                            .SingleInstance()
                            .OwnedByLifetimeScope();

                        var services =
                            ServiceRegistrationHelper.FindFromAssembly<PluginServiceImplementationAttribute>(assembly,
                                m_Logger);

                        var servicesRegistrations = services.OrderBy(d => d.Priority,
                            new PriorityComparer(PriortyComparisonMode.LowestFirst));

                        foreach (var servicesRegistration in servicesRegistrations)
                        {
                            var implementationType = servicesRegistration.ServiceImplementationType;
                            containerBuilder.RegisterType(implementationType)
                                .As(implementationType)
                                .WithLifetime(servicesRegistration.Lifetime)
                                .OwnedByLifetimeScope();

                            foreach (var service in servicesRegistration.ServiceTypes)
                            {
                                containerBuilder.Register(c => c.Resolve(implementationType))
                                    .As(service)
                                    .WithLifetime(servicesRegistration.Lifetime)
                                    .OwnedByLifetimeScope();
                            }
                        }

                        foreach (var type in pluginType.Assembly.FindTypes<IPluginContainerConfigurator>())
                        {
                            var configurator = (IPluginContainerConfigurator)ActivatorUtilitiesEx.CreateInstance(m_LifetimeScope, type);
                            configurator.ConfigureContainer(new PluginServiceConfigurationContext(m_LifetimeScope, configuration, containerBuilder));
                        }

                        var configurationEvent = new PluginContainerConfiguringEvent(pluginMetadata, pluginType,
                            configuration, containerBuilder, workingDirectory);
                        AsyncHelper.RunSync(() => m_EventBus.EmitAsync(m_Runtime, this, configurationEvent));
                    });

                    pluginInstance = (IOpenModPlugin)lifetimeScope.Resolve(pluginType);

                    var pluginLoggerType = typeof(ILogger<>).MakeGenericType(pluginType);
                    var pluginLogger = (ILogger)pluginInstance.LifetimeScope.Resolve(pluginLoggerType);

                    RegisterConfigChangeCallback(pluginInstance, pluginLogger);

                    var pluginActivateEvent = new PluginActivatingEvent(pluginInstance);
                    await m_EventBus.EmitAsync(m_Runtime, this, pluginActivateEvent);

                    if (pluginActivateEvent.IsCancelled)
                    {
                        await lifetimeScope.DisposeAsync();
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    m_Logger.LogError(ex, "Failed to load plugin from type: {PluginName} in assembly: {AssemblyName}",
                        pluginType.FullName, assembly.FullName);
                    return null;
                }

                try
                {
                    await pluginInstance.LoadAsync();
                    var serviceProvider = pluginInstance.LifetimeScope.Resolve<IServiceProvider>();
                    var pluginHelpWriter = ActivatorUtilities.CreateInstance<PluginHelpWriter>(serviceProvider);
                    await pluginHelpWriter.WriteHelpFileAsync();
                }
                catch (Exception ex)
                {
                    m_Logger.LogError(ex, "Failed to load plugin: {PluginName} v{PluginVersion}",
                        pluginInstance.DisplayName, pluginInstance.Version);

                    try
                    {
                        await pluginInstance.LifetimeScope.DisposeAsync();
                    }
                    catch (Exception e)
                    {
                        m_Logger.LogError(e, "Failed to unload plugin: {DisplayName} v{Version}", pluginInstance.DisplayName, pluginInstance.Version);
                    }

                    return null;
                }

                m_ActivatedPlugins.Add(new WeakReference(pluginInstance));
                return pluginInstance;
            }
            catch (Exception ex)
            {
                m_Logger.LogError(ex, "Failed to load plugin from assembly: {AssemblyName}", assembly.FullName);
                return null;
            }
        }

        private void RegisterConfigChangeCallback(IOpenModPlugin pluginInstance, ILogger pluginLogger)
        {
            var pluginConfiguration = pluginInstance.LifetimeScope.Resolve<IConfiguration>();

            var ignored = false;
            pluginConfiguration.GetReloadToken().RegisterChangeCallback(_ =>
            {
                if (ignored || !pluginInstance.IsComponentAlive)
                {
                    return;
                }

                AsyncHelper.Schedule($"Configuration changed event for {pluginInstance.OpenModComponentId}", () =>
                {
                    pluginLogger.LogInformation("Configuration updated for plugin: {ComponentId}",
                        pluginInstance.OpenModComponentId);
                    return m_EventBus.EmitAsync(m_Runtime, this, new PluginConfigurationChangedEvent(pluginInstance, pluginConfiguration));
                });

                RegisterConfigChangeCallback(pluginInstance, pluginLogger);
                ignored = true;
            }, null);
        }

        public async ValueTask DisposeAsync()
        {
            if (m_IsDisposing)
            {
                return;
            }

            m_IsDisposing = true;
            m_Logger.LogInformation("Unloading all plugins...");

            var i = 0;
            foreach (var instance in from plugin in m_ActivatedPlugins
                                     where plugin.IsAlive
                                     select (IOpenModPlugin)plugin.Target)
            {
                try
                {
                    await instance.LifetimeScope.DisposeAsync();
                    i++;
                }
                catch (Exception ex)
                {
                    m_Logger.LogError(ex, "An exception occured while unloading {DisplayName}", instance.DisplayName);
                }
            }

            m_ActivatedPlugins.Clear();
            m_Logger.LogInformation("> {Count} plugins unloaded", i);
        }
    }
}