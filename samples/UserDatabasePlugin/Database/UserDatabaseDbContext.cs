using System;
using Microsoft.EntityFrameworkCore;
using OpenMod.API;
using OpenMod.EntityFrameworkCore;

namespace UserDatabasePlugin.Database
{
    public class UserDatabaseDbContext : OpenModPluginDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }
        public UserDatabaseDbContext(DbContextOptions options, IServiceProvider serviceProvider) : base(options, serviceProvider)
        {
        }
    }
}