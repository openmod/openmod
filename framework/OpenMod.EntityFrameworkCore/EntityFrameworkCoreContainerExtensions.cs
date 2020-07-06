using System;
using System.Reflection;
using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Plugins;

namespace OpenMod.EntityFrameworkCore
{
    public static class EntityFrameworkCoreContainerExtensions
    {
        public static ContainerBuilder AddDbContext<T>(this ContainerBuilder containerBuilder) where T: OpenModDbContext
        {
            return AddDbContextInternal(containerBuilder, typeof(T), null, null);
        }

        public static ContainerBuilder AddDbContext<T>(this ContainerBuilder containerBuilder, ServiceLifetime serviceLifetime) where T : OpenModDbContext
        {
            return AddDbContextInternal(containerBuilder, typeof(T), null, serviceLifetime);
        }

        public static ContainerBuilder AddDbContext<T>(this ContainerBuilder containerBuilder, Action<DbContextOptionsBuilder> optionsBuilder) where T : OpenModDbContext
        {
            return AddDbContextInternal(containerBuilder, typeof(T), optionsBuilder, null);
        }

        public static ContainerBuilder AddDbContext<T>(this ContainerBuilder containerBuilder, Action<DbContextOptionsBuilder> optionsBuilder, ServiceLifetime serviceLifetime) where T : OpenModDbContext
        {
            return AddDbContextInternal(containerBuilder, typeof(T), optionsBuilder, serviceLifetime);
        }

        public static ContainerBuilder AddDbContext(this ContainerBuilder containerBuilder, Type dbContextType)
        {
            return AddDbContextInternal(containerBuilder, dbContextType, null, null);
        }

        public static ContainerBuilder AddDbContext(this ContainerBuilder containerBuilder, Type dbContextType, ServiceLifetime serviceLifetime)
        {
            return AddDbContextInternal(containerBuilder, dbContextType, null, serviceLifetime);
        }

        public static ContainerBuilder AddDbContext(this ContainerBuilder containerBuilder, Type dbContextType, Action<DbContextOptionsBuilder> optionsBuilder)
        {
            return AddDbContextInternal(containerBuilder, dbContextType, optionsBuilder, null);
        }

        public static ContainerBuilder AddDbContext(this ContainerBuilder containerBuilder, Type dbContextType, Action<DbContextOptionsBuilder> optionsBuilder, ServiceLifetime serviceLifetime)
        {
            return AddDbContextInternal(containerBuilder, dbContextType, optionsBuilder, serviceLifetime);
        }

        private static ContainerBuilder AddDbContextInternal(this ContainerBuilder containerBuilder, 
            Type dbContextType, 
            Action<DbContextOptionsBuilder> optionsBuilder, 
            ServiceLifetime? serviceLifetime)
        {
            serviceLifetime ??= ServiceLifetime.Transient;
            var builder = (DbContextOptionsBuilder) Activator.CreateInstance(typeof(DbContextOptionsBuilder<>).MakeGenericType(dbContextType));

            optionsBuilder?.Invoke(builder);

            containerBuilder.RegisterType<ConfigurationBasedConnectionStringAccessor>()
                .As<ConfigurationBasedConnectionStringAccessor>()
                .As<IConnectionStringAccessor>()
                .OwnedByLifetimeScope()
                .InstancePerDependency();

            var regBuilder = containerBuilder
                .RegisterType(dbContextType)
                .WithParameter("options", builder.Options)
                .OwnedByLifetimeScope();

            switch (serviceLifetime)
            {
                case ServiceLifetime.Singleton:
                    regBuilder.SingleInstance();
                    break;
                case ServiceLifetime.Scoped:
                    regBuilder.InstancePerLifetimeScope();
                    break;
                case ServiceLifetime.Transient:
                    regBuilder.InstancePerDependency();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(serviceLifetime), serviceLifetime, null);
            }

            return containerBuilder;
        }
    }
}