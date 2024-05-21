using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using OpenMod.EntityFrameworkCore.Configurator;
using Pomelo.EntityFrameworkCore.MySql.Storage;

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
            var serverVersion = ServerVersion.AutoDetect(connectionString);

            optionsBuilder.UseMySql(connectionString!, serverVersion,
                options =>
                {
                    options.MigrationsHistoryTable(dbContext.MigrationsTableName);
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
