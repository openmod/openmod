using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.EntityFrameworkCore.Extensions;

namespace OpenMod.EntityFrameworkCore
{
    /// <summary>
    /// Boilerplate code for design time context factories. Must be implemented to support EF Core commands.
    /// </summary>
    /// <typeparam name="TDbContext">The DbContext the factory is for.</typeparam>
    /// <example>
    /// <code>
    /// public class MyDbContextFactory : OpenModDbContextFactory&lt;MyDbContext&gt;
    /// {
    ///    // that's all needed
    /// }
    /// </code>
    /// </example>
    public class OpenModDbContextFactory<TDbContext> : IDesignTimeDbContextFactory<TDbContext> where TDbContext : OpenModDbContext<TDbContext>
    {
        public TDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddYamlFile("config.yaml", optional: false)
                .Build();

            var addDbContextMethod = typeof(EntityFrameworkServiceCollectionExtensions)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .FirstOrDefault(method =>
                {
                    var parameters = method.GetParameters();
                    return method.Name.Equals("AddDbContext", StringComparison.OrdinalIgnoreCase)
                           && method.IsGenericMethod && method.GetGenericArguments().Length == 1
                           && parameters.Length == 4
                           && parameters[0].ParameterType == typeof(IServiceCollection)
                           && parameters[1].ParameterType == typeof(Action<DbContextOptionsBuilder>)
                           && parameters[2].ParameterType == typeof(ServiceLifetime)
                           && parameters[3].ParameterType == typeof(ServiceLifetime);
                });

            if (addDbContextMethod == null)
            {
                throw new Exception("addDbContextMethod was null");
            }

            addDbContextMethod = addDbContextMethod.MakeGenericMethod(typeof(TDbContext));

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            serviceCollection.AddEntityFrameworkMySQL();
            serviceCollection.AddSingleton(config);
            serviceCollection.AddSingleton<IConfiguration>(config);
            serviceCollection.AddTransient<IConnectionStringAccessor, ConfigurationBasedConnectionStringAccessor>();
            addDbContextMethod.Invoke(null, new object[] { serviceCollection, null, ServiceLifetime.Scoped, ServiceLifetime.Scoped });
           
            var serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider.GetRequiredService<TDbContext>();
        }
    }
}