using System;
using Autofac;
using Microsoft.Extensions.Configuration;
using OpenMod.API.Plugins;
using OpenMod.EntityFrameworkCore.Extensions;
using UserDatabasePlugin.Database;

namespace UserDatabasePlugin
{
    public class ContainerConfigurator : IPluginContainerConfigurator
    {
        public void ConfigureContainer(
            ILifetimeScope parentLifetimeScope, 
            IConfiguration configuration,
            ContainerBuilder containerBuilder)
        {
            containerBuilder.AddEntityFrameworkCoreMySql();
            containerBuilder.AddDbContext<UserDatabaseDbContext>();
        }
    }
}