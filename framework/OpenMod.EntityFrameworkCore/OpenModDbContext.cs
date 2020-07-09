using System;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API.Plugins;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace OpenMod.EntityFrameworkCore
{
    public abstract class OpenModDbContext<TSelf>: DbContext where TSelf : OpenModDbContext<TSelf>
    {
        private readonly IServiceProvider m_ServiceProvider;
        private readonly ILogger<OpenModDbContext<TSelf>> m_Logger;

        protected OpenModDbContext([NotNull] DbContextOptions<TSelf> options, IServiceProvider serviceProvider) : base(options)
        {
            m_ServiceProvider = serviceProvider;
            m_Logger = serviceProvider.GetRequiredService<ILogger<OpenModDbContext<TSelf>>>();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            m_Logger.LogWarning("**** OnConfiguring");
            base.OnConfiguring(optionsBuilder);

            if (optionsBuilder.IsConfigured)
            {
                return;
            }

            var dbContextType = typeof(TSelf);
            var connectionStringAccessor = m_ServiceProvider.GetRequiredService<IConnectionStringAccessor>();
            var componentId = dbContextType.Assembly.GetCustomAttribute<PluginMetadataAttribute>().Id;
            var migrationTableName = "__" + componentId.Replace(".", "_") + "_MigrationsHistory";
            var connectionStringName = dbContextType.GetCustomAttribute<ConnectionStringAttribute>()?.Name ?? ConnectionStrings.Default;
            var connectionString = connectionStringAccessor.GetConnectionString(connectionStringName);

            m_Logger.LogWarning("**** UseMySql");
            optionsBuilder.UseMySql(connectionString, options =>
            {
                options.CharSetBehavior(CharSetBehavior.AppendToAllColumns);
                options.MigrationsHistoryTable(migrationTableName);
            });
        }
    }
}