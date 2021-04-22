using System;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace OpenMod.Core.Ioc.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> WithLifetime<TActivatorData, TRegistrationStyle>(
            this IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> registrationBuilder,
            ServiceLifetime serviceLifetime)
        {
            switch (serviceLifetime)
            {
                case ServiceLifetime.Singleton:
                    registrationBuilder.SingleInstance();
                    break;
                case ServiceLifetime.Scoped:
                    registrationBuilder.InstancePerLifetimeScope();
                    break;
                case ServiceLifetime.Transient:
                    registrationBuilder.InstancePerDependency();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(serviceLifetime), serviceLifetime, null);
            }

            return registrationBuilder;
        }

        public static ContainerBuilder PopulateServices(this ContainerBuilder containerBuilder,
            ServiceCollection serviceCollection,
            Func<ServiceDescriptor, bool>? serviceFilter = null,
            bool replaceServices = false,
            bool autowire = true)
        {
            foreach (var descriptor in serviceCollection)
            {
                if (descriptor.ServiceType == typeof(IServiceProvider)
                    || descriptor.ServiceType == typeof(IServiceScopeFactory))
                {
                    continue;
                }

                if (serviceFilter != null && !serviceFilter(descriptor))
                {
                    continue;
                }

                if (descriptor.ImplementationType != null)
                {
                    Log.Information("Descriptor: impl {ServiceType}, {ImplementationType}",
                        descriptor.ServiceType, descriptor.ImplementationType);

                    // Test if the an open generic type is being registered
                    var serviceTypeInfo = descriptor.ServiceType.GetTypeInfo();
                    if (serviceTypeInfo.IsGenericTypeDefinition)
                    {
                        var registrator = containerBuilder
                            .RegisterGeneric(descriptor.ImplementationType)
                            .As(descriptor.ServiceType)
                            .WithLifetime(descriptor.Lifetime);

                        if (autowire)
                        {
                            registrator.PropertiesAutowired();
                        }

                        if (!replaceServices)
                        {
                            registrator.IfNotRegistered(descriptor.ServiceType);
                        }
                    }
                    else
                    {
                        var registrator = containerBuilder
                            .RegisterType(descriptor.ImplementationType)
                            .As(descriptor.ServiceType)
                            .WithLifetime(descriptor.Lifetime);
                        if (!replaceServices)
                        {
                            registrator.IfNotRegistered(descriptor.ServiceType);
                        }

                    }
                }
                else if (descriptor.ImplementationFactory != null)
                {
                    var registrator = RegistrationBuilder.ForDelegate(descriptor.ServiceType, (context, _) =>
                        {
                            var serviceProvider = context.Resolve<IServiceProvider>();
                            return descriptor.ImplementationFactory(serviceProvider);
                        })
                        .WithLifetime(descriptor.Lifetime);

                    if (autowire)
                    {
                        registrator.PropertiesAutowired();
                    }

                    var registration = registrator.CreateRegistration();
                    containerBuilder.RegisterComponent(registration);
                }
                else
                {
                    Log.Information("Descriptor: instance {ServiceType}, {ImplementationType}",
                        descriptor.ServiceType, descriptor.ImplementationInstance.GetType());
                    var registrator = containerBuilder
                        .RegisterInstance(descriptor.ImplementationInstance)
                        .As(descriptor.ServiceType)
                        .WithLifetime(descriptor.Lifetime);

                    if (autowire)
                    {
                        registrator.PropertiesAutowired();
                    }

                    if (!replaceServices)
                    {
                        registrator.IfNotRegistered(descriptor.ServiceType);
                    }
                }
            }

            return containerBuilder;
        }
    }
}