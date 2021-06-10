using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace OpenMod.EntityFrameworkCore.MySql
{
    /// <summary>
    /// Base <see cref="DbContext"/> for OpenMod plugins.
    /// </summary>
    /// <typeparam name="TSelf">A type parameter pointing to the class implementing itself.</typeparam>
    /// <example>
    /// <code>
    /// public class MyDbContext : OpenModMySqlDbContext&lt;MyDbContext&gt;
    /// {
    ///     // impl
    /// }
    /// </code>
    /// </example>
    public abstract class OpenModMySqlDbContext<TSelf> : OpenModDbContextBase<TSelf>
        where TSelf : OpenModMySqlDbContext<TSelf>
    {
        private readonly IServiceProvider m_ServiceProvider;
        private readonly ILogger<TSelf> m_Logger;

        protected OpenModMySqlDbContext(IServiceProvider serviceProvider)
        {
            m_ServiceProvider = serviceProvider;
            m_Logger = serviceProvider.GetRequiredService<ILogger<TSelf>>();
        }

        protected OpenModMySqlDbContext(DbContextOptions<TSelf> options, IServiceProvider serviceProvider) :
            base(options)
        {
            m_ServiceProvider = serviceProvider;
            m_Logger = serviceProvider.GetRequiredService<ILogger<TSelf>>();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (optionsBuilder.IsConfigured)
            {
                return;
            }

            var connectionStringName = GetConnectionStringName();
            var connectionStringAccessor = m_ServiceProvider.GetRequiredService<IConnectionStringAccessor>();
            var connectionString = connectionStringAccessor.GetConnectionString(connectionStringName);

            optionsBuilder.UseMySql(connectionString,
                options => { options.MigrationsHistoryTable(MigrationsTableName); });
        }

        protected override void ApplyTableNameConvention(ModelBuilder modelBuilder)
        {
            if (string.IsNullOrEmpty(TablePrefix))
            {
                return;
            }

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var name = TablePrefix + entityType.GetTableName();
                m_Logger.LogDebug("Applying table name convention: {TableName} -> {NewTableName}",
                    entityType.GetTableName(), name);
                entityType.SetTableName(name);
            }
        }
    }
}
