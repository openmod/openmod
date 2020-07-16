using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace OpenMod.EntityFrameworkCore.Extensions
{
    public static class MigrationExtensions
    {
        public static async Task OpenModMigrateAsync<TDbContext>(this TDbContext dbContext) where TDbContext : OpenModDbContext<TDbContext>
        {
            // hotfix MySql.Data exception when the database exists but the migration table does not
            if (dbContext.Database.ProviderName.Equals("MySql.Data.EntityFrameworkCore", StringComparison.Ordinal))
            {
                var tableName = dbContext.MigrationsTableName;
                await dbContext.Database.ExecuteSqlRawAsync($"CREATE TABLE IF NOT EXISTS `{tableName}` ( `MigrationId` nvarchar(150) NOT NULL, `ProductVersion` nvarchar(32) NOT NULL, PRIMARY KEY (`MigrationId`) );");
            }

            await dbContext.Database.MigrateAsync();
        }
    }
}