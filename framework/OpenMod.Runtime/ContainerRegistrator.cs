using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Util;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.Core.Helpers.Prioritization;
using Serilog;

namespace OpenMod.Runtime
{
    public class ContainerRegistrator
    {
        private readonly ContainerBuilder m_ContainerBuilder;
        private readonly List<ServiceRegistration> m_ServiceRegistrations;
        public ContainerRegistrator(
            ContainerBuilder containerBuilder)
        {
            m_ContainerBuilder = containerBuilder;
            m_ServiceRegistrations = new List<ServiceRegistration>();
        }

        public void RegisterServicesFromAssembly(Assembly assembly)
        {
            m_ServiceRegistrations.AddRange(GetServicesFromAssembly(assembly));
        }

        public void CompleteRegistrations()
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

        public void BootstrapAndRegisterPlugins()
        {
            // todo: load all plugin assemblies first
            // todo: register all plugin assemblies, instantiate instances
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
    }

    internal class ServiceRegistration
    {
        public Type ServiceImplementationType { get; set; }

        public Type[] ServiceTypes { get; set; }

        public ServiceImplementationAttribute ServiceImplementationAttribute { get; set; }
    }

}