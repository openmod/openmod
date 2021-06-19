using Microsoft.EntityFrameworkCore;

namespace OpenMod.EntityFrameworkCore.Configurator
{
    public interface IDbContextConfigurator
    {
        void Configure<TDbContext>(OpenModDbContext<TDbContext> dbContext, DbContextOptionsBuilder optionsBuilder)
            where TDbContext : OpenModDbContext<TDbContext>;

        void Configure<TDbContext>(OpenModDbContext<TDbContext> dbContext, ModelBuilder modelBuilder)
            where TDbContext : OpenModDbContext<TDbContext>;
    }
}
