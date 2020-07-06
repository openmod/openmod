using System;
using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OpenMod.EntityFrameworkCore
{
    public static class EntityFrameworkCoreContainerExtensions
    {
        public static ContainerBuilder AddDbContext<T>(this ContainerBuilder containerBuilder, IConfiguration configuration, string connectionStringName) where T: PluginDbContext
        {
            return AddDbContext(containerBuilder, typeof(T), null, null, configuration, connectionStringName);
        }

        public static ContainerBuilder AddDbContext<T>(this ContainerBuilder containerBuilder, ServiceLifetime serviceLifetime, IConfiguration configuration, string connectionStringName) where T : PluginDbContext
        {
            return AddDbContext(containerBuilder, typeof(T), null, serviceLifetime, configuration, connectionStringName);
        }

        public static ContainerBuilder AddDbContext<T>(this ContainerBuilder containerBuilder, Action<DbContextOptionsBuilder> optionsBuilder, IConfiguration configuration, string connectionStringName) where T : PluginDbContext
        {
            return AddDbContext(containerBuilder, typeof(T), optionsBuilder, null, configuration, connectionStringName);
        }

        public static ContainerBuilder AddDbContext<T>(this ContainerBuilder containerBuilder, Action<DbContextOptionsBuilder> optionsBuilder) where T : PluginDbContext
        {
            return AddDbContext(containerBuilder, typeof(T), optionsBuilder, null, null, null);
        }

        public static ContainerBuilder AddDbContext<T>(this ContainerBuilder containerBuilder, Action<DbContextOptionsBuilder> optionsBuilder, ServiceLifetime serviceLifetime) where T : PluginDbContext
        {
            return AddDbContext(containerBuilder, typeof(T), optionsBuilder, serviceLifetime, null, null);
        }

        public static ContainerBuilder AddDbContext(this ContainerBuilder containerBuilder, Type dbContextType, IConfiguration configuration, string connectionStringName)
        {
            return AddDbContext(containerBuilder, dbContextType, null, null, configuration, connectionStringName);
        }

        public static ContainerBuilder AddDbContext(this ContainerBuilder containerBuilder, Type dbContextType, ServiceLifetime serviceLifetime, IConfiguration configuration, string connectionStringName)
        {
            return AddDbContext(containerBuilder, dbContextType, null, serviceLifetime, configuration, connectionStringName);
        }

        public static ContainerBuilder AddDbContext(this ContainerBuilder containerBuilder, Type dbContextType, Action<DbContextOptionsBuilder> optionsBuilder, IConfiguration configuration, string connectionStringName)
        {
            return AddDbContext(containerBuilder, dbContextType, optionsBuilder, null, configuration, connectionStringName);
        }
        
        public static ContainerBuilder AddDbContext(this ContainerBuilder containerBuilder, Type dbContextType, Action<DbContextOptionsBuilder> optionsBuilder)
        {
            return AddDbContext(containerBuilder, dbContextType, optionsBuilder, null, null, null);
        }

        public static ContainerBuilder AddDbContext(this ContainerBuilder containerBuilder, Type dbContextType, Action<DbContextOptionsBuilder> optionsBuilder, ServiceLifetime serviceLifetime)
        {
            return AddDbContext(containerBuilder, dbContextType, optionsBuilder, serviceLifetime, null, null);
        }

        public static ContainerBuilder AddDbContext(this ContainerBuilder containerBuilder, 
            Type dbContextType, 
            Action<DbContextOptionsBuilder> optionsBuilder, 
            ServiceLifetime? serviceLifetime,
            IConfiguration configuration,
            string connectionStringName)
        {
            serviceLifetime ??= ServiceLifetime.Transient;
            var builder = new DbContextOptionsBuilder();

            if (configuration != null)
            {
                DefaultDbContextConfigurer.Configure(builder, configuration.GetConnectionString(connectionStringName));
            }

            optionsBuilder?.Invoke(builder);

            var regBuilder =  containerBuilder
                .RegisterType(dbContextType)
                .WithParameter("options", builder.Options);

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