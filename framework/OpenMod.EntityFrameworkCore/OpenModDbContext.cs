using System;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Plugins;

namespace OpenMod.EntityFrameworkCore
{
    public abstract class OpenModDbContext : DbContext
    {
        private readonly IConnectionStringAccessor m_ConnectionStringAccessor;

        public virtual string ConnectionStringName { get; } = "default";

        protected OpenModDbContext([NotNull] DbContextOptions options, IServiceProvider serviceProvider) : base(options)
        {
            m_ConnectionStringAccessor = serviceProvider.GetRequiredService<IConnectionStringAccessor>();
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            base.OnConfiguring(options);

            var componentId = GetType().Assembly.GetCustomAttribute<PluginMetadataAttribute>().Id;
            string migrationTableName = "__" + componentId.Replace(".", "_") + "_MigrationsHistory";
            options.UseMySql(
                m_ConnectionStringAccessor.GetConnectionString(ConnectionStringName),
                x => x.MigrationsHistoryTable(migrationTableName));
        }
    }
}