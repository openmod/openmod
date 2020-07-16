using System;
using Microsoft.EntityFrameworkCore;
using OpenMod.EntityFrameworkCore;

namespace UserDatabasePlugin.Database
{
    public class UserDatabaseDbContext : OpenModDbContext<UserDatabaseDbContext>
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }

        public UserDatabaseDbContext(DbContextOptions<UserDatabaseDbContext> options, IServiceProvider serviceProvider) : base(options, serviceProvider)
        {
        }
    }
}