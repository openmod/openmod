using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace OpenMod.EntityFrameworkCore
{
    public abstract class OpenModDbContext<TSelf>: DbContext where TSelf : OpenModDbContext<TSelf>
    {

        protected OpenModDbContext([NotNull] DbContextOptions<TSelf> options) : base(options)
        {

        }
    }
}