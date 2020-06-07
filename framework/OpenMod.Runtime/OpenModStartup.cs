using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Util;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Plugins;
using OpenMod.Core.Helpers;
using OpenMod.Core.Helpers.Prioritization;
using OpenMod.Core.Plugins;
using OpenMod.Core.Plugins.NuGet;
using OpenMod.Core.Prioritization;
using OpenMod.NuGet;

namespace OpenMod.Runtime
{
    public class OpenModStartup : IOpenModStartup
    {
        private readonly IRuntime m_Runtime;
        private readonly IOpenModStartupContext m_OpenModStartupContext;
        private readonly NuGetPackageManager m_NuGetPackageManager;
        private readonly ContainerBuilder m_ContainerBuilder;
        private readonly List<ServiceRegistration> m_ServiceRegistrations;
        private readonly HashSet<AssemblyName> m_RegisteredAssemblies;
        private readonly PluginAssemblyStore m_PluginAssemblyStore;
        private readonly ILogger<OpenModStartup> m_Logger;

        public OpenModStartup(
            IOpenModStartupContext openModStartupContext,
            NuGetPackageManager nuGetPackageManager,
            ContainerBuilder containerBuilder)
        {
            m_OpenModStartupContext = openModStartupContext;
            m_NuGetPackageManager = nuGetPackageManager;
            m_Logger = openModStartupContext.LoggerFactory.CreateLogger<OpenModStartup>();
            m_Runtime = openModStartupContext.Runtime;
            m_ContainerBuilder = containerBuilder;
            m_ServiceRegistrations = new List<ServiceRegistration>();
            m_RegisteredAssemblies = new HashSet<AssemblyName>();
            m_PluginAssemblyStore = new PluginAssemblyStore(openModStartupContext.LoggerFactory.CreateLogger<PluginAssemblyStore>());

        }

        public void RegisterServiceFromAssemblyWithResources(Assembly assembly, string relativeDir)
        {
            RegisterServicesFromAssembly(assembly);
            if (string.IsNullOrWhiteSpace(relativeDir))
            {
                relativeDir = string.Empty;
            }

            AssemblyHelper.CopyAssemblyResources(assembly, Path.Combine(m_Runtime.WorkingDirectory, relativeDir));
        }

        public void RegisterServicesFromAssembly(Assembly assembly)
        {
            var assemblyName = assembly.GetName();
            if (m_RegisteredAssemblies.Contains(assembly.GetName()))
            {
                m_Logger.LogDebug("Skipping already registered assembly: " + assemblyName);
                return;
            }

            m_ServiceRegistrations.AddRange(GetServicesFromAssembly(assembly));
            m_RegisteredAssemblies.Add(assemblyName);
        }

        public async Task RegisterPluginAssembliesAsync(IPluginAssembliesSource source)
        {
            var assemblies = await m_PluginAssemblyStore.LoadPluginAssembliesAsync(source);
            foreach (var assembly in assemblies)
            {
                // PluginAssemblyStore checks if this attribute exists
                var pluginMetadata = assembly.GetCustomAttribute<PluginMetadataAttribute>();
                var pluginDirectory = PluginHelper.GetWorkingDirectory(m_Runtime, pluginMetadata.Id);

                if (!Directory.Exists(pluginDirectory))
                {
                    Directory.CreateDirectory(pluginDirectory);
                }

                AssemblyHelper.CopyAssemblyResources(assembly, Path.Combine(m_Runtime.WorkingDirectory, "plugins", pluginMetadata.Id));
            }

            foreach (var assembly in assemblies)
            {
                // Auto register services with [Service] and [ServiceImplementation] attributes
                RegisterServicesFromAssembly(assembly);

                // Auto create IContainerConfigurator classes for more advanced scenarios
                var containerConfiugratorTypes = assembly.FindTypes<IContainerConfigurator>(false);
                foreach (var containerConfiguratorType in containerConfiugratorTypes)
                {
                    var instance = (IContainerConfigurator)Activator.CreateInstance(containerConfiguratorType);
                    await instance.ConfigureContainerAsync(m_OpenModStartupContext, m_ContainerBuilder);
                }
            }
        }

        internal void Complete()
        {
            var servicesRegistrations = m_ServiceRegistrations
                .OrderBy(d => d.ServiceImplementationAttribute.Priority, new PriorityComparer(PriortyComparisonMode.LowestFirst));

            foreach (var servicesRegistration in servicesRegistrations)
            {
                var builder = m_ContainerBuilder.RegisterType(servicesRegistration.ServiceImplementationType);
                foreach (var serviceType in servicesRegistration.ServiceTypes)
                {
                    builder.As(serviceType);

                    switch (servicesRegistration.ServiceImplementationAttribute.Lifetime)
                    {
                        case ServiceLifetime.Singleton:
                            builder.SingleInstance();
                            break;
                        case ServiceLifetime.Scoped:
                            builder.InstancePerLifetimeScope();
                            break;
                        case ServiceLifetime.Transient:
                            builder.InstancePerDependency();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        internal async Task BootstrapAndRegisterPluginsAsync()
        {
            var pluginsDirectory = Path.Combine(m_Runtime.WorkingDirectory, "plugins");

            var fileSystemPluginAssembliesSource = new FileSystemPluginAssembliesSource(pluginsDirectory);
            m_ContainerBuilder.Register(context => fileSystemPluginAssembliesSource)
                .AsSelf()
                .SingleInstance()
                .OwnedByLifetimeScope();
            
            var nugetPluginAssembliesSource = new NuGetPluginAssembliesSource(m_NuGetPackageManager);
            m_ContainerBuilder.Register(context => nugetPluginAssembliesSource)
                .AsSelf()
                .SingleInstance()
                .OwnedByLifetimeScope();

            await RegisterPluginAssembliesAsync(fileSystemPluginAssembliesSource);
            await RegisterPluginAssembliesAsync(nugetPluginAssembliesSource);

            m_ContainerBuilder.Register(context => m_PluginAssemblyStore)
                .As<IPluginAssemblyStore>()
                .SingleInstance()
                .OwnedByLifetimeScope();
        }

        internal async Task ConfigureServicesAsync(IServiceCollection services)
        {
            foreach (var assembly in m_PluginAssemblyStore.LoadedPluginAssemblies)
            {
                // Auto create IContainerConfigurator classes for more advanced scenarios
                var containerConfiugratorTypes = assembly.FindTypes<IServiceConfigurator>(false);
                foreach (var containerConfiguratorType in containerConfiugratorTypes)
                {
                    var instance = (IServiceConfigurator)Activator.CreateInstance(containerConfiguratorType);
                    await instance.ConfigureServicesAsync(m_OpenModStartupContext, services);
                }
            }
        }

        private IEnumerable<ServiceRegistration> GetServicesFromAssembly(Assembly assembly)
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
                    ServiceImplementationType = type,
                    ServiceTypes = interfaces,
                    ServiceImplementationAttribute = attribute
                };
            }
        }
    }

    internal class ServiceRegistration
    {
        public Type ServiceImplementationType { get; set; }

        public Type[] ServiceTypes { get; set; }

        public ServiceImplementationAttribute ServiceImplementationAttribute { get; set; }
    }

}