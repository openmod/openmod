using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace OpenMod.EntityFrameworkCore
{
    public abstract class OpenModDbContext : DbContext
    {

        protected OpenModDbContext([NotNull] DbContextOptions options) : base(options)
        {

        }
    }
}