using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Persistence;
using OpenMod.API.Plugins;
using OpenMod.API.Prioritization;
using OpenMod.Core.Helpers;
using OpenMod.Core.Ioc;
using OpenMod.Core.Ioc.Extensions;
using OpenMod.Core.Localization;
using OpenMod.Core.Prioritization;

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

            var pluginTypes = assembly.FindTypes<IOpenModPlugin>().ToList();
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
                assembly.GetTypes(); //this will cause a exception if some libs are missing, somehow is not catched in PluginAssemblyStore, we need to catched it here or otherise it will throw a exception at 'ServiceRegistrationHelper.FindFromAssembly'


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
                        .As<IOpenModComponent>()
                        .As<IOpenModPlugin>()
                        .SingleInstance()
                        .ExternallyOwned();

                    containerBuilder.Register(context => m_DataStoreFactory.CreateDataStore(null, workingDirectory))
                        .As<IDataStore>()
                        .SingleInstance()
                        .OwnedByLifetimeScope();

                    var stringLocalizer = Directory.Exists(workingDirectory)
                        ? m_StringLocalizerFactory.Create("translations", workingDirectory)
                        : NullStringLocalizer.Instance;

                    containerBuilder.Register(context => stringLocalizer)
                        .As<IStringLocalizer>()
                        .SingleInstance()
                        .OwnedByLifetimeScope();

                    var services = ServiceRegistrationHelper.FindFromAssembly<PluginServiceImplementationAttribute>(assembly, m_Logger);

                    var servicesRegistrations = services.OrderBy(d => d.Priority, new PriorityComparer(PriortyComparisonMode.LowestFirst));

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
                        var configurator = (IPluginContainerConfigurator)ActivatorUtilities.CreateInstance(serviceProvider, type);
                        configurator.ConfigureContainer(m_LifetimeScope, configuration, containerBuilder);
                    }
                });

                pluginInstance = (IOpenModPlugin)lifetimeScope.Resolve(pluginType);
            }
            catch (ReflectionTypeLoadException refEx)
            {
                var missingTypes = new List<string>();
                for (var i = 0; i < refEx.LoaderExceptions.Length; i++)
                {
                    if (!(refEx.LoaderExceptions[i] is TypeLoadException typeEx))
                        continue;

                    var msgSplitted = typeEx.Message.Split(',');
                    for (var j = msgSplitted.Length - 1; j > 0; j--)
                    {
                        /*
                        System.TypeLoadException: Could not load type of field 'OpenMod.Economy.Helpers.MySqlHelper+<>c__DisplayClass3_0:action' (1) due to:
                        Could not resolve type with token 01000028 (from typeref, class/assembly MySqlConnector.MySqlCommand, MySqlConnector, Version=1.0.0.0, Culture=neutral, PublicKeyToken=d33d3e53aa5f8c92) 
                        assembly:MySqlConnector, Version=1.0.0.0, Culture=neutral, PublicKeyToken=d33d3e53aa5f8c92 type:MySqlConnector.MySqlCommand member:(null) signature:<none>
                        
                        We want to locate 'assembly:MySqlConnector, Version=1.0.0.0'*/
                        if (!msgSplitted[j].TrimStart().StartsWith("Version=", StringComparison.OrdinalIgnoreCase))
                            continue;

                        var packageName = msgSplitted[j - 1].Split(':').Last();
                        if (!missingTypes.Contains(packageName))
                            missingTypes.Add(packageName);

                        break;
                    }
                }

                if (missingTypes.Count > 0)
                {
                    m_Logger.LogError($"Some libraries are missing for plugin \"{pluginMetadata.Id}\"");
                    m_Logger.LogError($"Missing type: {string.Join(", ", missingTypes)}.");
                    m_Logger.LogInformation("Try install from: \"openmod install 'type'\"");
                    return null;
                }

                m_Logger.LogError($"Failed to load plugin from type: {pluginType.FullName} in assembly: {assembly.FullName}");
                m_Logger.LogError(refEx, $"Failed to load some types from plugin \"{pluginMetadata.Id}\"");
                if (refEx.LoaderExceptions == null || refEx.LoaderExceptions.Length == 0) 
                    return null;

                foreach (var loaderException in refEx.LoaderExceptions)
                {
                    m_Logger.LogError(loaderException, "Loader Exception: ");
                }

                return null;
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