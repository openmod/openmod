using Autofac;
using Microsoft.Extensions.Configuration;
using OpenMod.API.Plugins;
using OpenMod.EntityFrameworkCore;
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
            containerBuilder.AddDbContext<UserDatabaseDbContext>();
        }
    }
}