using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace OpenMod.EntityFrameworkCore
{
    public abstract class PluginDbContext : DbContext
    {
        protected PluginDbContext()
        {
            
        }

        protected PluginDbContext([NotNull] DbContextOptions options) : base(options)
        {

        }
    }
}