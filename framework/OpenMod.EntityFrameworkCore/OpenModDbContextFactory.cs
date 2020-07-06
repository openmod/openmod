using System;
using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Plugins;

namespace OpenMod.EntityFrameworkCore
{
    public class OpenModDbContextFactory<TDbContext> : IDesignTimeDbContextFactory<TDbContext> where TDbContext : OpenModDbContext
    {
        public TDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddYamlFile("config.yaml", optional: false)
                .Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(config);
            serviceCollection.AddSingleton<IConfiguration>(config);
            serviceCollection.AddTransient<IConnectionStringAccessor, ConfigurationBasedConnectionStringAccessor>();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var dbContextType = typeof(TDbContext);
            var connectionStringName = dbContextType.GetCustomAttribute<ConnectionStringAttribute>()?.Name ?? ConnectionStrings.Default;
            var connectionStringAccessor = serviceProvider.GetRequiredService<IConnectionStringAccessor>();
            var connectionString = connectionStringAccessor.GetConnectionString(connectionStringName);
            var componentId = dbContextType.Assembly.GetCustomAttribute<PluginMetadataAttribute>().Id;
            var migrationTableName = "__" + componentId.Replace(".", "_") + "_MigrationsHistory";

            var optionsBuilder = (DbContextOptionsBuilder)Activator.CreateInstance(typeof(DbContextOptionsBuilder<>).MakeGenericType(dbContextType));
            optionsBuilder.UseMySQL(connectionString, x => x.MigrationsHistoryTable(migrationTableName));

            optionsBuilder.UseApplicationServiceProvider(serviceProvider);
            return (TDbContext) Activator.CreateInstance(typeof(TDbContext), optionsBuilder.Options);
        }
    }
}