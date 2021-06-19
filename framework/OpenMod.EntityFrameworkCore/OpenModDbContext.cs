using Microsoft.EntityFrameworkCore;
using OpenMod.API.Plugins;
using OpenMod.EntityFrameworkCore.Configurator;
using System;
using System.Reflection;

namespace OpenMod.EntityFrameworkCore
{
    public abstract class OpenModDbContext<TSelf> : DbContext where TSelf : OpenModDbContext<TSelf>
    {
        internal readonly IServiceProvider ServiceProvider;
        private readonly IDbContextConfigurator? m_DbContextConfigurator;

        protected OpenModDbContext(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        protected OpenModDbContext(IDbContextConfigurator configurator, IServiceProvider serviceProvider)
        {
            m_DbContextConfigurator = configurator;
            ServiceProvider = serviceProvider;
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            m_DbContextConfigurator?.Configure(this, optionsBuilder);
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            m_DbContextConfigurator?.Configure(this, modelBuilder);
        }

        /// <summary>
        /// Gets the name of the migrations table for supporting providers.
        /// </summary>
        protected internal virtual string MigrationsTableName
        {
            get
            {
                var componentId = GetType().Assembly.GetCustomAttribute<PluginMetadataAttribute>().Id;
                return "__" + componentId.Replace(".", "_") + "_MigrationsHistory".ToLower();
            }
        }

        /// <summary>
        /// Gets the prefix for the tables for supporting providers.
        /// </summary>
        protected internal virtual string? TablePrefix
        {
            get
            {
                var componentId = GetType().Assembly.GetCustomAttribute<PluginMetadataAttribute>()?.Id ??
                                  throw new InvalidOperationException("Could not find plugin metadata");

                return componentId.Replace(".", "_") + "_";
            }
        }

        /// <summary>
        /// Gets the name of the connection string used by supporting providers.
        /// </summary>
        protected internal virtual string GetConnectionStringName()
        {
            return typeof(TSelf).GetCustomAttribute<ConnectionStringAttribute>()?.Name ??
                   ConnectionStrings.Default;
        }
    }
}