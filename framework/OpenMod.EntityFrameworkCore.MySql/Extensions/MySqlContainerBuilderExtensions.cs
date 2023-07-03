using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.EntityFrameworkCore.Extensions;
using OpenMod.EntityFrameworkCore.MySql.Configurator;
using System;

namespace OpenMod.EntityFrameworkCore.MySql.Extensions
{
    public static class MySqlContainerBuilderExtensions
    {
        public static ContainerBuilder AddMySqlDbContext<T>(this ContainerBuilder containerBuilder) where T : OpenModDbContext<T>
        {
            return AddMySqlDbContextInternal(containerBuilder, typeof(T), null, null);
        }

        public static ContainerBuilder AddMySqlDbContext<T>(this ContainerBuilder containerBuilder, ServiceLifetime serviceLifetime) where T : OpenModDbContext<T>
        {
            return AddMySqlDbContextInternal(containerBuilder, typeof(T), null, serviceLifetime);
        }

        public static ContainerBuilder AddMySqlDbContext<T>(this ContainerBuilder containerBuilder, Action<DbContextOptionsBuilder>? optionsBuilder) where T : OpenModDbContext<T>
        {
            return AddMySqlDbContextInternal(containerBuilder, typeof(T), optionsBuilder, null);
        }

        public static ContainerBuilder AddMySqlDbContext<T>(this ContainerBuilder containerBuilder, Action<DbContextOptionsBuilder>? optionsBuilder, ServiceLifetime serviceLifetime) where T : OpenModDbContext<T>
        {
            return AddMySqlDbContextInternal(containerBuilder, typeof(T), optionsBuilder, serviceLifetime);
        }

        public static ContainerBuilder AddMySqlDbContext(this ContainerBuilder containerBuilder, Type dbContextType)
        {
            return AddMySqlDbContextInternal(containerBuilder, dbContextType, null, null);
        }

        public static ContainerBuilder AddMySqlDbContext(this ContainerBuilder containerBuilder, Type dbContextType, ServiceLifetime serviceLifetime)
        {
            return AddMySqlDbContextInternal(containerBuilder, dbContextType, null, serviceLifetime);
        }

        public static ContainerBuilder AddMySqlDbContext(this ContainerBuilder containerBuilder, Type dbContextType, Action<DbContextOptionsBuilder>? optionsBuilder)
        {
            return AddMySqlDbContextInternal(containerBuilder, dbContextType, optionsBuilder, null);
        }

        public static ContainerBuilder AddMySqlDbContext(this ContainerBuilder containerBuilder, Type dbContextType, Action<DbContextOptionsBuilder>? optionsBuilder, ServiceLifetime serviceLifetime)
        {
            return AddMySqlDbContextInternal(containerBuilder, dbContextType, optionsBuilder, serviceLifetime);
        }

        private static ContainerBuilder AddMySqlDbContextInternal(this ContainerBuilder containerBuilder,
            Type dbContextType,
            Action<DbContextOptionsBuilder>? optionsBuilder,
            ServiceLifetime? serviceLifetime)
        {
            containerBuilder.RegisterType<ConfigurationBasedConnectionStringAccessor>()
                .As<ConfigurationBasedConnectionStringAccessor>()
                .As<IConnectionStringAccessor>()
                .OwnedByLifetimeScope()
                .InstancePerDependency();

            containerBuilder.RegisterType<PomeloMySqlConnectorResolver>()
                .AsSelf()
                .SingleInstance()
                .AutoActivate();

            containerBuilder.AddDbContext(dbContextType, optionsBuilder,
                serviceLifetime ?? ServiceLifetime.Transient, new MySqlDbContextConfigurator());

            return containerBuilder;
        }
    }
}
