using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OpenMod.API.Plugins;

namespace OpenMod.EntityFrameworkCore
{
    public abstract class OpenModDbContext<TSelf>: DbContext where TSelf : OpenModDbContext<TSelf>
    {
        protected OpenModDbContext([NotNull] DbContextOptions<TSelf> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (optionsBuilder.IsConfigured)
            {
                return;
            }

            var connectionStringAccessor = Database.GetService<IConnectionStringAccessor>();
            var componentId = GetType().Assembly.GetCustomAttribute<PluginMetadataAttribute>().Id;
            var migrationTableName = "__" + componentId.Replace(".", "_") + "_MigrationsHistory";
            var connectionStringName = GetType().GetCustomAttribute<ConnectionStringAttribute>()?.Name ?? ConnectionStrings.Default;
            var connectionString = connectionStringAccessor.GetConnectionString(connectionStringName);

            optionsBuilder.UseMySql(connectionString, x => x.MigrationsHistoryTable(migrationTableName));
        }
    }
}