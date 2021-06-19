using Microsoft.EntityFrameworkCore;
using OpenMod.API.Plugins;
using System;
using System.Reflection;

namespace OpenMod.EntityFrameworkCore
{
    public abstract class OpenModDbContext<TSelf> : DbContext where TSelf : OpenModDbContext<TSelf>
    {
        private readonly IServiceProvider m_ServiceProvider;

        protected OpenModDbContext(IServiceProvider serviceProvider)
        {
            m_ServiceProvider = serviceProvider;
        }

        protected OpenModDbContext(DbContextOptions<TSelf> options, IServiceProvider serviceProvider) :
            base(options)
        {
            m_ServiceProvider = serviceProvider;
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