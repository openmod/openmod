using System;
using Microsoft.EntityFrameworkCore;
using OpenMod.EntityFrameworkCore;
using OpenMod.EntityFrameworkCore.Configurator;

namespace UserDatabasePlugin.Database
{
    public class UserDatabaseDbContext : OpenModDbContext<UserDatabaseDbContext>
    {
        public UserDatabaseDbContext(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public UserDatabaseDbContext(IDbContextConfigurator configurator, IServiceProvider serviceProvider) : base(configurator, serviceProvider)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<UserActivity> UserActivities { get; set; } = null!;
    }
}