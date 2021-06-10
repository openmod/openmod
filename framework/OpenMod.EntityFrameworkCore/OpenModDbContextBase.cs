using Microsoft.EntityFrameworkCore;
using OpenMod.API.Plugins;
using System.Reflection;

namespace OpenMod.EntityFrameworkCore
{
    public abstract class OpenModDbContextBase<TSelf> : DbContext where TSelf : OpenModDbContextBase<TSelf>
    {
        /// <summary>
        /// Gets the name of the migrations table.
        /// </summary>
        public virtual string MigrationsTableName
        {
            get
            {
                var componentId = GetType().Assembly.GetCustomAttribute<PluginMetadataAttribute>().Id;
                return "__" + componentId.Replace(".", "_") + "_MigrationsHistory".ToLower();
            }
        }

        /// <summary>
        /// Gets the prefix for the tables.
        /// </summary>
        public virtual string TablePrefix
        {
            get
            {
                var componentId = GetType().Assembly.GetCustomAttribute<PluginMetadataAttribute>().Id;
                return componentId.Replace(".", "_") + "_";
            }
        }

        protected OpenModDbContextBase()
        {
        }

        protected OpenModDbContextBase(DbContextOptions<TSelf> options) : base(options)
        {
        }

        protected virtual string GetConnectionStringName()
        {
            return typeof(TSelf).GetCustomAttribute<ConnectionStringAttribute>()?.Name ??
                ConnectionStrings.Default;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ApplyTableNameConvention(modelBuilder);
        }

        protected virtual void ApplyTableNameConvention(ModelBuilder modelBuilder) { }
    }
}