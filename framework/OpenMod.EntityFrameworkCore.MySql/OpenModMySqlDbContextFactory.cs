using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.EntityFrameworkCore.MySql.Configurator;
using System;

namespace OpenMod.EntityFrameworkCore.MySql
{
    /// <summary>
    /// Boilerplate code for design time context factories. Must be implemented to support EF Core commands.
    /// </summary>
    /// <typeparam name="TDbContext">The DbContext the factory is for.</typeparam>
    /// <example>
    /// <code>
    /// public class MyDbContextFactory : OpenModMySqlDbContextFactory&lt;MyDbContext&gt;
    /// {
    ///    // that's all needed
    /// }
    /// </code>
    /// </example>
    public abstract class OpenModMySqlDbContextFactory<TDbContext> : IDesignTimeDbContextFactory<TDbContext>
        where TDbContext : OpenModDbContext<TDbContext>
    {
        public TDbContext CreateDbContext(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddYamlFile("config.yaml", optional: false)
                .Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            serviceCollection.AddSingleton(config);
            serviceCollection.AddSingleton<IConfiguration>(config);
            serviceCollection.AddTransient<IConnectionStringAccessor, ConfigurationBasedConnectionStringAccessor>();
            serviceCollection.AddEntityFrameworkMySql();

            serviceCollection.AddTransient(services =>
                (TDbContext)ActivatorUtilities.CreateInstance(services, typeof(TDbContext),
                    new MySqlDbContextConfigurator()));

            var serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider.GetRequiredService<TDbContext>();
        }
    }
}
