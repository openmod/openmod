using Microsoft.EntityFrameworkCore;
using System;

namespace OpenMod.EntityFrameworkCore.Configurator
{
    public class DbContextOptionsBuilderActionContextConfigurator : IDbContextConfigurator
    {
        private readonly Action<DbContextOptionsBuilder> m_OptionsBuilderAction;

        public DbContextOptionsBuilderActionContextConfigurator(Action<DbContextOptionsBuilder> optionsBuilderAction)
        {
            m_OptionsBuilderAction = optionsBuilderAction;
        }

        public void Configure<TDbContext>(OpenModDbContext<TDbContext> dbContext, DbContextOptionsBuilder optionsBuilder)
            where TDbContext : OpenModDbContext<TDbContext>
        {
            m_OptionsBuilderAction.Invoke(optionsBuilder);
        }

        public void Configure<TDbContext>(OpenModDbContext<TDbContext> dbContext, ModelBuilder modelBuilder)
            where TDbContext : OpenModDbContext<TDbContext>
        {
        }
    }
}
