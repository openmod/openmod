using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using OpenMod.EntityFrameworkCore.Configurator;

namespace OpenMod.EntityFrameworkCore.MySql.Configurator
{
    public class MySqlDbContextConfigurator : IDbContextConfigurator
    {
        public void Configure<TDbContext>(OpenModDbContext<TDbContext> dbContext,
            DbContextOptionsBuilder optionsBuilder) where TDbContext : OpenModDbContext<TDbContext>
        {
            var connectionStringName = dbContext.GetConnectionStringName();
            var connectionStringAccessor = dbContext.ServiceProvider.GetRequiredService<IConnectionStringAccessor>();
            var connectionString = connectionStringAccessor.GetConnectionString(connectionStringName);
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("ConnectionString cannot be null or white space.");
            }

            ServerVersion serverVersion;
            try
            {
                serverVersion = ServerVersion.AutoDetect(connectionString);
            }
            catch (MySqlException)
            {
                // if mysql server is not up, use default version instead
                serverVersion = ServerVersion.Parse("8.0.21");
            }

            optionsBuilder.UseMySql(connectionString, serverVersion, options =>
            {
                options.MigrationsHistoryTable(dbContext.MigrationsTableName);

                /* Fixes this exception:
                 * System.InvalidOperationException: The LINQ expression 'DbSet<Server>()
                 *       .Where(s => s.Instance.Equals(
                 *          value: __serverID_0,
                 *          comparisonType: Ordinal))' could not be translated.
                 *  
                 *  Translation of the 'String.Equals' overload with a 'StringComparison' parameter is not supported by default.
                 *  For general EF Core information about this error, see https://go.microsoft.com/fwlink/?linkid=2129535 for more information.
                 *  Either rewrite the query in a form that can be translated, or switch to client evaluation explicitly by inserting a call to 'AsEnumerable', 'AsAsyncEnumerable', 'ToList', or 'ToListAsync'.
                 *  See https://go.microsoft.com/fwlink/?linkid=2101038 for more information.
                 *  
                 *  In future this option will be removed.
                 */
                options.EnableStringComparisonTranslations(enable: true);
            });
        }

        public void Configure<TDbContext>(OpenModDbContext<TDbContext> dbContext, ModelBuilder modelBuilder)
            where TDbContext : OpenModDbContext<TDbContext>
        {
            if (string.IsNullOrEmpty(dbContext.TablePrefix))
            {
                return;
            }

            var logger = dbContext.ServiceProvider.GetRequiredService<ILogger<TDbContext>>();

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var name = dbContext.TablePrefix + entityType.GetTableName();
                logger.LogDebug("Applying table name convention: {TableName} -> {NewTableName}",
                    entityType.GetTableName(), name);
                entityType.SetTableName(name);
            }
        }
    }
}
