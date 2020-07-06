using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Plugins;

namespace OpenMod.EntityFrameworkCore.Extensions
{
    public static class EntityFrameworkCoreContainerExtensions
    {
        public static ContainerBuilder AddDbContext<T>(this ContainerBuilder containerBuilder) where T : OpenModDbContext<T>
        {
            return AddDbContextInternal(containerBuilder, typeof(T), null, null);
        }

        public static ContainerBuilder AddDbContext<T>(this ContainerBuilder containerBuilder, ServiceLifetime serviceLifetime) where T : OpenModDbContext<T>
        {
            return AddDbContextInternal(containerBuilder, typeof(T), null, serviceLifetime);
        }

        public static ContainerBuilder AddDbContext<T>(this ContainerBuilder containerBuilder, Action<DbContextOptionsBuilder> optionsBuilder) where T : OpenModDbContext<T>
        {
            return AddDbContextInternal(containerBuilder, typeof(T), optionsBuilder, null);
        }

        public static ContainerBuilder AddDbContext<T>(this ContainerBuilder containerBuilder, Action<DbContextOptionsBuilder> optionsBuilder, ServiceLifetime serviceLifetime) where T : OpenModDbContext<T>
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
            Action<DbContextOptionsBuilder> optionsBuilderAction,
            ServiceLifetime? serviceLifetime)
        {
            serviceLifetime ??= ServiceLifetime.Transient;
            containerBuilder.RegisterType<ConfigurationBasedConnectionStringAccessor>()
                .As<ConfigurationBasedConnectionStringAccessor>()
                .As<IConnectionStringAccessor>()
                .OwnedByLifetimeScope()
                .InstancePerDependency();

            ServiceCollection mysqlServices = new ServiceCollection();
            mysqlServices.AddEntityFrameworkMySql();
            containerBuilder.Populate(mysqlServices);

            var componentId = dbContextType.Assembly.GetCustomAttribute<PluginMetadataAttribute>().Id;
            var migrationTableName = "__" + componentId.Replace(".", "_") + "_MigrationsHistory";
            var connectionStringName = dbContextType.GetCustomAttribute<ConnectionStringAttribute>()?.Name ?? ConnectionStrings.Default;

            var regBuilder = containerBuilder
                .Register(context =>
                {
                    var connectionStringAccessor = context.Resolve<IConnectionStringAccessor>();
                    var connectionString = connectionStringAccessor.GetConnectionString(connectionStringName);

                    var optionsBuilder = (DbContextOptionsBuilder)Activator.CreateInstance(typeof(DbContextOptionsBuilder<>).MakeGenericType(dbContextType));
                    optionsBuilder.UseMySql(connectionString, x => x.MigrationsHistoryTable(migrationTableName));
                    optionsBuilderAction?.Invoke(optionsBuilder);

                    var serviceProivder = context.Resolve<IServiceProvider>();
                    optionsBuilder.UseApplicationServiceProvider(serviceProivder);
                    optionsBuilder.UseInternalServiceProvider(serviceProivder);

                    return ActivatorUtilities.CreateInstance(serviceProivder, dbContextType, optionsBuilder.Options);
                })
                .As(dbContextType)
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



