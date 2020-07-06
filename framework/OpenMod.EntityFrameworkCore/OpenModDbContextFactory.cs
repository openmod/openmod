using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OpenMod.EntityFrameworkCore
{
    public class OpenModDbContextFactory<TDbContext> : IDesignTimeDbContextFactory<TDbContext> where TDbContext : OpenModDbContext
    {
        public TDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TDbContext>();

            var config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddYamlFile("config.yaml", optional: false)
                .Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(config);
            serviceCollection.AddSingleton<IConfiguration>(config);
            serviceCollection.AddTransient<IConnectionStringAccessor, ConfigurationBasedConnectionStringAccessor>();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            return (TDbContext) Activator.CreateInstance(typeof(TDbContext), optionsBuilder.Options, serviceProvider);
        }
    }
}