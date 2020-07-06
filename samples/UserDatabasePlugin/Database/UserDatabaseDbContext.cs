using Microsoft.EntityFrameworkCore;
using OpenMod.API;
using OpenMod.EntityFrameworkCore;

namespace UserDatabasePlugin.Database
{
    public class UserDatabaseDbContext : OpenModDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }
        public UserDatabaseDbContext(DbContextOptions options, IOpenModComponent openModComponent) : base(options, openModComponent)
        {

        }
    }
}