using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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

        public static ContainerBuilder AddEntityFrameworkCore(this ContainerBuilder containerBuilder)
        {
            ServiceCollection mysqlServices = new ServiceCollection();
            mysqlServices.AddEntityFrameworkMySql();
            containerBuilder.Populate(mysqlServices);

            containerBuilder.RegisterType<ConfigurationBasedConnectionStringAccessor>()
                .As<ConfigurationBasedConnectionStringAccessor>()
                .As<IConnectionStringAccessor>()
                .OwnedByLifetimeScope()
                .InstancePerDependency();

            return containerBuilder;
        }

        private static ContainerBuilder AddDbContextInternal(this ContainerBuilder containerBuilder,
            Type dbContextType,
            Action<DbContextOptionsBuilder> optionsBuilderAction,
            ServiceLifetime? serviceLifetime)
        {
            serviceLifetime ??= ServiceLifetime.Scoped;

            ServiceCollection dbContextServiceCollection = new ServiceCollection();
            var addDbContextMethod = typeof(EntityFrameworkServiceCollectionExtensions)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .FirstOrDefault(method =>
                {
                    var parameters = method.GetParameters();
                    return method.Name.Equals("AddDbContext", StringComparison.OrdinalIgnoreCase)
                           && method.IsGenericMethod && method.GetGenericArguments().Length == 1
                           && parameters[0].ParameterType == typeof(IServiceCollection)
                           && parameters[1].ParameterType == typeof(Action<DbContextOptionsBuilder>)
                           && parameters[2].ParameterType == typeof(ServiceLifetime)
                           && parameters[3].ParameterType == typeof(ServiceLifetime);
                });
     
            if (addDbContextMethod == null)
            {
                throw new Exception("addDbContextMethod was null");
            }

            addDbContextMethod = addDbContextMethod.MakeGenericMethod(dbContextType);
            addDbContextMethod.Invoke(null,  new object[] { dbContextServiceCollection, optionsBuilderAction, serviceLifetime, ServiceLifetime.Scoped});

            containerBuilder.Populate(dbContextServiceCollection);
            return containerBuilder;
        }
    }
}



