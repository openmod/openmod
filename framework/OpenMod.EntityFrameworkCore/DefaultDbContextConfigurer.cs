using Microsoft.EntityFrameworkCore;

namespace OpenMod.EntityFrameworkCore
{
    public static class DefaultDbContextConfigurer
    {
        public static DbContextOptionsBuilder Configure(DbContextOptionsBuilder builder, string connectionString)
        {
            builder.UseMySql(connectionString);
            return builder;
        }
    }
}