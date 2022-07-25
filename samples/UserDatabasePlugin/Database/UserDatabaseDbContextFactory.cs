using JetBrains.Annotations;
using OpenMod.EntityFrameworkCore.MySql;

namespace UserDatabasePlugin.Database
{
    [UsedImplicitly]
    public class UserDatabaseDbContextFactory : OpenModMySqlDbContextFactory<UserDatabaseDbContext>
    {
        
    }
}