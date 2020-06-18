using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Plugins;
using OpenMod.API.Prioritization;
using OpenMod.Core.Helpers;
using OpenMod.Core.Plugins;
using OpenMod.Core.Plugins.NuGet;
using OpenMod.Core.Prioritization;
using OpenMod.NuGet;

namespace OpenMod.Runtime
{
    public class OpenModStartup : IOpenModStartup
    {
        private readonly IRuntime m_Runtime;
        private readonly NuGetPackageManager m_NuGetPackageManager;
        private readonly List<ServiceRegistration> m_ServiceRegistrations;
        private readonly HashSet<AssemblyName> m_RegisteredAssemblies;
        private readonly PluginAssemblyStore m_PluginAssemblyStore;
        private readonly ILogger<OpenModStartup> m_Logger;
        private readonly List<Assembly> m_Assemblies;
        private readonly List<IPluginAssembliesSource> m_PluginAssembliesSources;
        public OpenModStartup(IOpenModStartupContext openModStartupContext)
        {
            Context = openModStartupContext;
            m_NuGetPackageManager = ((OpenModStartupContext)openModStartupContext).NuGetPackageManager;
            m_Logger = openModStartupContext.LoggerFactory.CreateLogger<OpenModStartup>();
            m_Runtime = openModStartupContext.Runtime;
            m_Assemblies = new List<Assembly>();
            m_ServiceRegistrations = new List<ServiceRegistration>();
            m_RegisteredAssemblies = new HashSet<AssemblyName>();
            m_PluginAssemblyStore = new PluginAssemblyStore(openModStartupContext.LoggerFactory.CreateLogger<PluginAssemblyStore>());
            m_PluginAssembliesSources = new List<IPluginAssembliesSource>();
        }

        public IOpenModStartupContext Context { get; }

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
            if (m_RegisteredAssemblies.Contains(assembly.GetName()))
            {
                m_Logger.LogDebug("Skipping already registered assembly: " + assemblyName);
                return;
            }

            m_ServiceRegistrations.AddRange(GetServiceRegistrationsFromAssembly(assembly));
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
                var pluginMetadata = assembly.GetCustomAttribute<PluginMetadataAttribute>();
                AssemblyHelper.CopyAssemblyResources(assembly, Path.Combine(m_Runtime.WorkingDirectory, "plugins", pluginMetadata.Id));
            }

            foreach (var assembly in assemblies)
            {
                // Auto register services with [Service] and [ServiceImplementation] attributes
                RegisterServicesFromAssembly(assembly);
            }

            m_Assemblies.AddRange(assemblies);
            return assemblies;
        }

        internal async Task CompleteConfigurationAsync(IConfigurationBuilder builder)
        {
            var containerConfiguratorTypes = m_Assemblies
                .SelectMany(d => d.FindTypes<IConfigurationConfigurator>(false))
                .OrderBy(d => d.GetPriority(), new PriorityComparer(PriortyComparisonMode.LowestFirst));

            foreach (var configurationConfigurator in containerConfiguratorTypes)
            {
                var instance = (IConfigurationConfigurator)Activator.CreateInstance(configurationConfigurator);
                await instance.ConfigureConfigurationAsync(Context, builder);
            }
        }

        internal async Task CompleteContainerRegistrationAsync(ContainerBuilder containerBuilder)
        {
            var containerConfiguratorTypes = m_Assemblies
                .SelectMany(d => d.FindTypes<IContainerConfigurator>(false))
                .OrderBy(d => d.GetPriority(), new PriorityComparer(PriortyComparisonMode.LowestFirst));

            foreach (var containerConfiguratorType in containerConfiguratorTypes)
            {
                var instance = (IContainerConfigurator)Activator.CreateInstance(containerConfiguratorType);
                await instance.ConfigureContainerAsync(Context, containerBuilder);
            }
        }

        internal async Task CompleteServiceRegistrationsAsync(IServiceCollection serviceCollection)
        {
            var sortedSources = m_PluginAssembliesSources
                .OrderBy(d => d.GetType().GetCustomAttribute<ServiceImplementationAttribute>()?.Priority ?? Priority.Normal
                    , new PriorityComparer(PriortyComparisonMode.LowestFirst));

            foreach (var source in sortedSources)
            {
                var lifetime = source.GetType().GetCustomAttribute<ServiceImplementationAttribute>()?.Lifetime ?? ServiceLifetime.Singleton;
                serviceCollection.Add(new ServiceDescriptor(source.GetType(), source.GetType(), lifetime));
            }

            serviceCollection.AddSingleton<IPluginAssemblyStore>(m_PluginAssemblyStore);
            var serviceConfiguratorTypes = m_Assemblies
                .SelectMany(d => d.FindTypes<IServiceConfigurator>(false))
                .OrderBy(d => d.GetPriority(), new PriorityComparer(PriortyComparisonMode.LowestFirst));

            foreach (var serviceConfiguratorType in serviceConfiguratorTypes)
            {
                var instance = (IServiceConfigurator)Activator.CreateInstance(serviceConfiguratorType);
                await instance.ConfigureServicesAsync(Context, serviceCollection);
            }

            var servicesRegistrations = m_ServiceRegistrations.OrderBy(d => d.Priority, new PriorityComparer(PriortyComparisonMode.LowestFirst));

            foreach (var servicesRegistration in servicesRegistrations)
            {
                var implementationType = servicesRegistration.ServiceImplementationType;
                serviceCollection.Add(new ServiceDescriptor(implementationType, implementationType, servicesRegistration.Lifetime));

                foreach (var service in servicesRegistration.ServiceTypes)
                {
                    serviceCollection.Add(new ServiceDescriptor(service, provider => provider.GetService(implementationType), servicesRegistration.Lifetime));
                }
            }
        }

        internal async Task LoadPluginAssembliesAsync()
        {
            var pluginsDirectory = Path.Combine(m_Runtime.WorkingDirectory, "plugins");

            var fileSystemPluginAssembliesSource = new FileSystemPluginAssembliesSource(pluginsDirectory);
            await RegisterPluginAssembliesAsync(fileSystemPluginAssembliesSource);
            
            var nugetPluginAssembliesSource = new NuGetPluginAssembliesSource(m_NuGetPackageManager);
            await RegisterPluginAssembliesAsync(nugetPluginAssembliesSource);
        }

        private IEnumerable<ServiceRegistration> GetServiceRegistrationsFromAssembly(Assembly assembly)
        {
            var types = assembly.GetLoadableTypes()
                .Where(d => d.IsClass && !d.IsInterface && !d.IsAbstract)
                .ToList();

            foreach (var type in types)
            {
                var attribute = type.GetCustomAttribute<ServiceImplementationAttribute>();
                if (attribute == null)
                {
                    continue;
                }

                var interfaces = type.GetInterfaces()
                    .Where(d => d.GetCustomAttribute<ServiceAttribute>() != null)
                    .ToArray();

                if (interfaces.Length == 0)
                {
                    m_Logger.LogWarning($"Type {type.FullName} in assembly {assembly.FullName} has been marked as ServiceImplementation but does not inherit any services!\nDid you forget to add [Service] to your interfaces?");
                    continue;
                }

                yield return new ServiceRegistration
                {
                    Priority = attribute.Priority,
                    ServiceImplementationType = type,
                    ServiceTypes = interfaces,
                    Lifetime = attribute.Lifetime
                };
            }
        }
    }
}