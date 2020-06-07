using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Autofac;
using Autofac.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Yaml;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Plugins;
using OpenMod.Core.Helpers;
using OpenMod.Core.Helpers.Prioritization;
using OpenMod.Core.Plugins;
using OpenMod.Core.Plugins.NuGet;
using Serilog;

namespace OpenMod.Runtime
{
    public class ContainerRegistrator
    {
        private readonly IRuntime m_Runtime;
        private readonly ContainerBuilder m_ContainerBuilder;
        private readonly IConfigurationBuilder m_ConfigurationBuilder;
        private readonly List<ServiceRegistration> m_ServiceRegistrations;
        private readonly HashSet<AssemblyName> m_RegisteredAssemblies;
        private PluginAssemblyStore m_PluginAssemblyStore;

        public ContainerRegistrator(IRuntime runtime,
            ContainerBuilder containerBuilder, IConfigurationBuilder configurationBuilder)
        {
            m_Runtime = runtime;
            m_ContainerBuilder = containerBuilder;
            m_ConfigurationBuilder = configurationBuilder;
            m_ServiceRegistrations = new List<ServiceRegistration>();
            m_RegisteredAssemblies = new HashSet<AssemblyName>();
        }

        public void CopyAssemblyResources(Assembly assembly, string relativePath = null)
        {
            if (relativePath == null)
            {
                relativePath = string.Empty;
            }

            var resourceNames = assembly.GetManifestResourceNames();

            foreach (var resourceName in resourceNames)
            {
                var regex = new Regex(Regex.Escape(assembly.GetName().Name + "."));
                var fileName = regex.Replace(resourceName, string.Empty, 1);

                var filePath = Path.Combine(m_Runtime.WorkingDirectory, relativePath, fileName);
                if (File.Exists(filePath))
                {
                    continue;
                }

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string fileContent = reader.ReadToEnd();
                    File.WriteAllText(filePath, fileContent);
                }
            }
        }

        public void RegisterServicesFromAssembly(Assembly assembly)
        {
            var assemblyName = assembly.GetName();
            if (m_RegisteredAssemblies.Contains(assembly.GetName()))
            {
                Log.Debug("Skipping already registered assembly: " + assemblyName);
                return;
            }

            m_ServiceRegistrations.AddRange(GetServicesFromAssembly(assembly));
            m_RegisteredAssemblies.Add(assemblyName);
        }

        public void Complete()
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

        public async Task BootstrapAndRegisterPluginsAsync()
        {
            var pluginsDirectory = Path.Combine(m_Runtime.WorkingDirectory, "assemblies");
            if (!Directory.Exists(pluginsDirectory))
            {
                Directory.CreateDirectory(pluginsDirectory);
            }

            var packagesDirectory = Path.Combine(m_Runtime.WorkingDirectory, "packages");
            if (!Directory.Exists(packagesDirectory))
            {
                Directory.CreateDirectory(packagesDirectory);
            }

            m_PluginAssemblyStore = new PluginAssemblyStore(new List<PluginAssembliesSource>
            {
                new FileSystemPluginAssembliesSource(pluginsDirectory),
                new NuGetPluginAssembliesSource(packagesDirectory)
            });

            m_ContainerBuilder.Register(context => m_PluginAssemblyStore)
                .As<IPluginAssemblyStore>()
                .SingleInstance();

            await m_PluginAssemblyStore.LoadPluginAssembliesAsync();
            foreach (var assembly in m_PluginAssemblyStore.LoadedPluginAssemblies)
            {
                // PluginAssemblyStore checks if this attribute exists
                var pluginMetadata = assembly.GetCustomAttribute<PluginMetadataAttribute>();
                var pluginDirectory = PluginHelper.GetWorkingDirectory(m_Runtime, pluginMetadata.Id);

                if (!Directory.Exists(pluginDirectory))
                {
                    Directory.CreateDirectory(pluginDirectory);
                }

                CopyAssemblyResources(assembly, Path.Combine("plugins", pluginMetadata.Id));
            }

            foreach (var assembly in m_PluginAssemblyStore.LoadedPluginAssemblies)
            {
                // Auto register services with [Service] and [ServiceImplementation] attributes
                RegisterServicesFromAssembly(assembly);

                // Auto create IContainerConfigurator classes for more advanced scenarios
                var containerConfiugratorTypes = assembly.FindTypes<IContainerConfigurator>(false);
                foreach (var containerConfiguratorType in containerConfiugratorTypes)
                {
                    var instance = (IContainerConfigurator) Activator.CreateInstance(containerConfiguratorType);
                    instance.ConfigureContainer(m_Runtime, m_ContainerBuilder);
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
                    Log.Warning($"Type {type.FullName} in assembly {assembly.FullName} has been marked as ServiceImplementation but does not inherit any services!\nDid you forget to add [Service] to your interfaces?");
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

        public void ConfigureServices(IServiceCollection services)
        {
            foreach (var assembly in m_PluginAssemblyStore.LoadedPluginAssemblies)
            {
                // Auto create IContainerConfigurator classes for more advanced scenarios
                var containerConfiugratorTypes = assembly.FindTypes<IServiceConfigurator>(false);
                foreach (var containerConfiguratorType in containerConfiugratorTypes)
                {
                    var instance = (IServiceConfigurator)Activator.CreateInstance(containerConfiguratorType);
                    instance.ConfigureServices(m_Runtime, services);
                }
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