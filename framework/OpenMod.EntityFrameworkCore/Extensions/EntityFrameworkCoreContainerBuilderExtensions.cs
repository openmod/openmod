using System;
using System.Reflection;
using Autofac;
using HarmonyLib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using OpenMod.Core.Ioc.Extensions;
using OpenMod.EntityFrameworkCore.Compatibility;
using Serilog;

namespace OpenMod.EntityFrameworkCore.Extensions
{
    public static class EntityFrameworkCoreContainerBuilderExtensions
    {
        private static bool s_MonoFixApplied;

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

        public static ContainerBuilder AddEntityFrameworkCoreMySql(this ContainerBuilder containerBuilder)
        {
            //if (!s_MonoFixApplied && Type.GetType("Mono.Runtime") != null)
            //{
            //    var mysqlConnectorAssembly = typeof(MySqlConnection).Assembly;
            //    var type = mysqlConnectorAssembly.GetType("MySqlConnector.Utilities.SocketExtensions");
            //    var setBufferMethod = type?.GetMethod("SetBuffer", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            //    if (setBufferMethod == null)
            //    {
            //        Log.Warning("Failed to apply MySqlConnector Mono patch.");
            //        Log.Warning("Failed to apply MySqlConnector Mono patch.");
            //    }
            //    else
            //    {
            //        var patchMethod = typeof(MySqlConnectorMonoFix).GetMethod("SetBuffer", BindingFlags.Public | BindingFlags.Static);
            //        var harmony = new Harmony("com.get-openmod.entityframeworkcore");
            //        harmony.Patch(setBufferMethod, new HarmonyMethod(patchMethod));
            //    }

            //    s_MonoFixApplied = true;
            //}

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

            containerBuilder
                .Register(context =>
                {
                    var optionsBuilder = (DbContextOptionsBuilder)Activator.CreateInstance(typeof(DbContextOptionsBuilder<>).MakeGenericType(dbContextType));
                    optionsBuilder.UseLoggerFactory(context.Resolve<ILoggerFactory>());

                    var applicationServiceProvider = context.Resolve<IServiceProvider>();
                    optionsBuilderAction?.Invoke(optionsBuilder);
                    return Activator.CreateInstance(dbContextType, optionsBuilder.Options, applicationServiceProvider);
                })
                .As(dbContextType)
                .WithLifetime(serviceLifetime.Value)
                .OwnedByLifetimeScope();

            return containerBuilder;
        }
    }

    public class ServiceDisposer
    {
        public ServiceDisposer(IDisposable disposable)
        {
            
        }
    }
}