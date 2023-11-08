﻿using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NuGet.Packaging;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Plugins;
using OpenMod.API.Prioritization;
using OpenMod.Common.Helpers;
using OpenMod.Core.Helpers;
using OpenMod.Core.Ioc;
using OpenMod.Core.Ioc.Extensions;
using OpenMod.Core.Plugins;
using OpenMod.Core.Plugins.NuGet;
using OpenMod.Core.Prioritization;
using OpenMod.NuGet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace OpenMod.Runtime
{
    [OpenModInternal]
    public class OpenModStartup : IOpenModStartup
    {
        private readonly IRuntime m_Runtime;
        private readonly List<ServiceRegistration> m_ServiceRegistrations;
        private readonly HashSet<AssemblyName> m_RegisteredAssemblies;
        private readonly PluginAssemblyStore m_PluginAssemblyStore;
        private readonly ILogger<OpenModStartup> m_Logger;
        private readonly HashSet<Assembly> m_Assemblies;
        private readonly List<IPluginAssembliesSource> m_PluginAssembliesSources;
        private readonly NuGetPackageManager m_NuGetPackageManager;

        public OpenModStartup(IOpenModServiceConfigurationContext openModStartupContext)
        {
            Context = openModStartupContext;
            m_NuGetPackageManager = ((OpenModStartupContext)openModStartupContext).NuGetPackageManager;
            m_Logger = openModStartupContext.LoggerFactory.CreateLogger<OpenModStartup>();
            m_Runtime = openModStartupContext.Runtime;
            m_Assemblies = new HashSet<Assembly>(new AssemblyEqualityComparer());
            m_ServiceRegistrations = new List<ServiceRegistration>();
            m_RegisteredAssemblies = new HashSet<AssemblyName>();

            var logger = openModStartupContext.LoggerFactory.CreateLogger<PluginAssemblyStore>();
            m_PluginAssemblyStore = new PluginAssemblyStore(logger, m_NuGetPackageManager);
            m_PluginAssembliesSources = new List<IPluginAssembliesSource>();
        }

        public IOpenModServiceConfigurationContext Context { get; }

        public void RegisterIocAssemblyAndCopyResources(Assembly assembly, string assemblyDir)
        {
            RegisterServicesFromAssembly(assembly);
            if (string.IsNullOrWhiteSpace(assemblyDir))
            {
                assemblyDir = string.Empty;
            }

            AssemblyHelper.CopyAssemblyResources(assembly, Path.Combine(m_Runtime.WorkingDirectory, assemblyDir));
        }

        public void RegisterServicesFromAssembly(Assembly assembly)
        {
            var assemblyName = assembly.GetName();
            if (m_RegisteredAssemblies.Contains(assemblyName))
            {
                m_Logger.LogDebug("Skipping already registered assembly: {AssemblyName}", assemblyName);
                return;
            }

            m_ServiceRegistrations.AddRange(
                ServiceRegistrationHelper.FindFromAssembly<ServiceImplementationAttribute>(assembly, m_Logger));
            m_RegisteredAssemblies.Add(assemblyName);
            m_Assemblies.Add(assembly);
        }

        public async Task<ICollection<Assembly>> RegisterPluginAssembliesAsync(IPluginAssembliesSource source)
        {
            m_PluginAssembliesSources.Add(source);
            var assemblies = await m_PluginAssemblyStore.LoadPluginAssembliesAsync(source);
            foreach (var assembly in assemblies)
            {
                // PluginAssemblyStore checks if this attribute exists
                try
                {
                    var pluginMetadata = assembly.GetCustomAttribute<PluginMetadataAttribute>();
                    AssemblyHelper.CopyAssemblyResources(assembly,
                        Path.Combine(m_Runtime.WorkingDirectory, "plugins", pluginMetadata.Id));
                }
                catch (Exception ex)
                {
                    m_Logger.LogError(ex, "Failed to copy resources from assembly: {Assembly}", assembly);
                }
            }

            foreach (var assembly in assemblies)
            {
                // Auto register services with [Service] and [ServiceImplementation] attributes
                try
                {
                    RegisterServicesFromAssembly(assembly);
                }
                catch (Exception ex)
                {
                    m_Logger.LogError(ex, "Failed to load services from assembly: {Assembly}", assembly);
                }
            }

            m_Assemblies.AddRange(assemblies);
            return assemblies;
        }

        internal void ConfigureConfiguration(IConfigurationBuilder builder)
        {
            var containerConfiguratorTypes = m_Assemblies
                .SelectMany(d => d.FindTypes<IConfigurationConfigurator>())
                .OrderBy(d => d.GetPriority(), new PriorityComparer(PriortyComparisonMode.LowestFirst));

            foreach (var configurationConfiguratorType in containerConfiguratorTypes)
            {
                try
                {
                    var instance = (IConfigurationConfigurator)Activator.CreateInstance(configurationConfiguratorType);
                    instance.ConfigureConfiguration(Context, builder);
                }
                catch (Exception ex)
                {
                    m_Logger.LogError(ex, "Failed to configure configuration from: {ConfiguratorType}",
                        configurationConfiguratorType.FullName);
                }
            }
        }

        internal void SetupContainer(ContainerBuilder containerBuilder)
        {
            foreach (var servicesRegistration in m_ServiceRegistrations
                .OrderBy(d => d.Priority, new PriorityComparer(PriortyComparisonMode.LowestFirst)))
            {
                containerBuilder.RegisterType(servicesRegistration.ServiceImplementationType)
                    .AsSelf()
                    .As(servicesRegistration.ServiceTypes)
                    .WithLifetime(servicesRegistration.Lifetime)
                    .PropertiesAutowired();
            }

            var containerConfiguratorTypes = m_Assemblies
                .SelectMany(d => d.FindTypes<IContainerConfigurator>())
                .OrderBy(d => d.GetPriority(), new PriorityComparer(PriortyComparisonMode.LowestFirst));

            foreach (var containerConfiguratorType in containerConfiguratorTypes)
            {
                try
                {
                    var instance = (IContainerConfigurator)Activator.CreateInstance(containerConfiguratorType);
                    instance.ConfigureContainer(Context, containerBuilder);
                }
                catch (Exception ex)
                {
                    m_Logger.LogError(ex, "Failed to configure container from: {ConfiguratorType}",
                        containerConfiguratorType.FullName);
                }
            }
        }

        internal void SetupServices(IServiceCollection serviceCollection)
        {
            var sourceServices = new SortedSet<ServiceRegistration>(new PriorityComparer(PriortyComparisonMode.LowestFirst));
            m_PluginAssembliesSources.ForEach(source =>
            {
                var sourceType = source.GetType();
                var serviceImplementationAttribute = sourceType.GetCustomAttribute<ServiceImplementationAttribute>();

                var priority = serviceImplementationAttribute?.Priority ?? Priority.Normal;
                var serviceRegistration = new ServiceRegistration()
                {
                    ServiceImplementationType = sourceType,
                    Priority = priority,
                    Lifetime = serviceImplementationAttribute?.Lifetime ?? ServiceLifetime.Singleton
                };

                sourceServices.Add(serviceRegistration);
            });

            serviceCollection.Add(sourceServices.Select(s => new ServiceDescriptor(s.ServiceImplementationType, s.ServiceImplementationType, s.Lifetime)));

            serviceCollection.AddSingleton<IPluginAssemblyStore>(m_PluginAssemblyStore);
            var serviceConfiguratorTypes = m_Assemblies
                .SelectMany(d => d.FindTypes<IServiceConfigurator>())
                .OrderBy(d => d.GetPriority(), new PriorityComparer(PriortyComparisonMode.LowestFirst));

            foreach (var serviceConfiguratorType in serviceConfiguratorTypes)
            {
                try
                {
                    var instance = (IServiceConfigurator)Activator.CreateInstance(serviceConfiguratorType);
                    instance.ConfigureServices(Context, serviceCollection);
                }
                catch (Exception ex)
                {
                    m_Logger.LogError(ex, "Failed to configure services from: {ConfiguratorType}",
                        serviceConfiguratorType.FullName);
                }
            }
        }

        internal async Task LoadPluginAssembliesAsync()
        {
            try
            {
                await RegisterPluginAssembliesAsync(new NuGetPluginAssembliesSource(m_NuGetPackageManager));
            }
            catch (Exception ex)
            {
                m_Logger.LogError(ex, "Failed to load NuGet plugins");
            }

            try
            {
                var pluginsDirectory = Path.Combine(m_Runtime.WorkingDirectory, "plugins");
                var logger = Context.LoggerFactory.CreateLogger<FileSystemPluginAssembliesSource>();
                var fileSystemPluginAssembliesSource = new FileSystemPluginAssembliesSource(logger, pluginsDirectory);
                await RegisterPluginAssembliesAsync(fileSystemPluginAssembliesSource);
            }
            catch (Exception ex)
            {
                m_Logger.LogError(ex, "Failed to load .dll plugins");
            }
        }
    }
}