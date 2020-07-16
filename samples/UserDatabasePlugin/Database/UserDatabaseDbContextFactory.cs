using JetBrains.Annotations;
using OpenMod.EntityFrameworkCore;

namespace UserDatabasePlugin.Database
{
    [UsedImplicitly]
    public class UserDatabaseDbContextFactory : OpenModDbContextFactory<UserDatabaseDbContext>
    {
        
    }
}