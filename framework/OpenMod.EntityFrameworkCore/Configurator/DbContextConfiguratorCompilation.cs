using Microsoft.EntityFrameworkCore;
using MoreLinq;
using System.Collections.Generic;

namespace OpenMod.EntityFrameworkCore.Configurator
{
    public class DbContextConfiguratorCompilation : IDbContextConfigurator
    {
        private readonly ICollection<IDbContextConfigurator> m_DbContextConfigurators;

        public DbContextConfiguratorCompilation(params IDbContextConfigurator[] configurators)
        {
            m_DbContextConfigurators = configurators;
        }

        public void Configure<TDbContext>(OpenModDbContext<TDbContext> dbContext, DbContextOptionsBuilder optionsBuilder)
            where TDbContext : OpenModDbContext<TDbContext>
        {
            m_DbContextConfigurators.ForEach(x => x.Configure(dbContext, optionsBuilder));
        }

        public void Configure<TDbContext>(OpenModDbContext<TDbContext> dbContext, ModelBuilder modelBuilder)
            where TDbContext : OpenModDbContext<TDbContext>
        {
            m_DbContextConfigurators.ForEach(x => x.Configure(dbContext, modelBuilder));
        }
    }
}
