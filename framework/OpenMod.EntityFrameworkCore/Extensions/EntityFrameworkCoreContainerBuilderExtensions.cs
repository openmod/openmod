using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.Core.Ioc.Extensions;
using OpenMod.EntityFrameworkCore.Configurator;
using System;

namespace OpenMod.EntityFrameworkCore.Extensions
{
    public static class EntityFrameworkCoreContainerBuilderExtensions
    {
        public static ContainerBuilder AddDbContext<T>(this ContainerBuilder containerBuilder) where T : OpenModDbContext<T>
        {
            return AddDbContextInternal(containerBuilder, typeof(T), null, null, null);
        }

        public static ContainerBuilder AddDbContext<T>(this ContainerBuilder containerBuilder, ServiceLifetime serviceLifetime) where T : OpenModDbContext<T>
        {
            return AddDbContextInternal(containerBuilder, typeof(T), null, serviceLifetime, null);
        }

        public static ContainerBuilder AddDbContext<T>(this ContainerBuilder containerBuilder, Action<DbContextOptionsBuilder>? optionsBuilder) where T : OpenModDbContext<T>
        {
            return AddDbContextInternal(containerBuilder, typeof(T), optionsBuilder, null, null);
        }

        public static ContainerBuilder AddDbContext<T>(this ContainerBuilder containerBuilder, Action<DbContextOptionsBuilder>? optionsBuilder, ServiceLifetime serviceLifetime) where T : OpenModDbContext<T>
        {
            return AddDbContextInternal(containerBuilder, typeof(T), optionsBuilder, serviceLifetime, null);
        }

        public static ContainerBuilder AddDbContext(this ContainerBuilder containerBuilder, Type dbContextType)
        {
            return AddDbContextInternal(containerBuilder, dbContextType, null, null, null);
        }

        public static ContainerBuilder AddDbContext(this ContainerBuilder containerBuilder, Type dbContextType, ServiceLifetime serviceLifetime)
        {
            return AddDbContextInternal(containerBuilder, dbContextType, null, serviceLifetime, null);
        }

        public static ContainerBuilder AddDbContext(this ContainerBuilder containerBuilder, Type dbContextType, Action<DbContextOptionsBuilder>? optionsBuilder)
        {
            return AddDbContextInternal(containerBuilder, dbContextType, optionsBuilder, null, null);
        }

        public static ContainerBuilder AddDbContext(this ContainerBuilder containerBuilder, Type dbContextType, Action<DbContextOptionsBuilder>? optionsBuilder, ServiceLifetime serviceLifetime)
        {
            return AddDbContextInternal(containerBuilder, dbContextType, optionsBuilder, serviceLifetime, null);
        }

        public static ContainerBuilder AddDbContext(this ContainerBuilder containerBuilder, Type dbContextType, Action<DbContextOptionsBuilder>? optionsBuilder, ServiceLifetime serviceLifetime, IDbContextConfigurator configurator)
        {
            return AddDbContextInternal(containerBuilder, dbContextType, optionsBuilder, serviceLifetime, configurator);
        }

        private static ContainerBuilder AddDbContextInternal(this ContainerBuilder containerBuilder,
            Type dbContextType,
            Action<DbContextOptionsBuilder>? optionsBuilderAction,
            ServiceLifetime? serviceLifetime,
            IDbContextConfigurator? configurator)
        {
            serviceLifetime ??= ServiceLifetime.Transient;

            containerBuilder
                .Register(context =>
                {
                    var applicationServiceProvider = context.Resolve<IServiceProvider>();
                    var loggerFactory = context.Resolve<ILoggerFactory>();

                    void Builder(DbContextOptionsBuilder builder)
                    {
                        builder.UseLoggerFactory(loggerFactory);
                        builder.UseApplicationServiceProvider(applicationServiceProvider);
                        optionsBuilderAction?.Invoke(builder);
                    }

                    var actionConfigurator = new DbContextOptionsBuilderActionContextConfigurator(Builder);

                    IDbContextConfigurator compilationConfigurator = configurator == null
                        ? actionConfigurator
                        : new DbContextConfiguratorCompilation(actionConfigurator, configurator);

                    return ActivatorUtilities.CreateInstance(
                        applicationServiceProvider, dbContextType, compilationConfigurator);
                })
                .PropertiesAutowired()
                .As(dbContextType)
                .WithLifetime(serviceLifetime.Value)
                .OwnedByLifetimeScope();

            return containerBuilder;
        }
    }
}