using Autofac;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OpenMod.API;

namespace OpenMod.EntityFrameworkCore
{
    public abstract class OpenModDbContext : DbContext
    {
        public IConfiguration Configuration { get; set; }
        public IOpenModComponent OpenModComponent { get; }
        public virtual string ConnectionStringName { get; } = "default";

        protected OpenModDbContext([NotNull] DbContextOptions options, [NotNull] IOpenModComponent openModComponent) : base(options)
        {
            OpenModComponent = openModComponent;
            Configuration = openModComponent.LifetimeScope.Resolve<IConfiguration>();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string migrationTableName = "__" + OpenModComponent.OpenModComponentId.Replace(".", "_") + "_MigrationsHistory";
            options.UseMySql(
                GetConnectionString(),
                x => x.MigrationsHistoryTable(migrationTableName));
        }

        protected virtual string GetConnectionString()
        {
            return Configuration.GetConnectionString(ConnectionStringName);
        }
    }
}