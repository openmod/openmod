using OpenMod.API.Plugins;
using OpenMod.EntityFrameworkCore.Extensions;
using UserDatabasePlugin.Database;

namespace UserDatabasePlugin
{
    public class ContainerConfigurator : IPluginContainerConfigurator
    {
        public void ConfigureContainer(IPluginServiceConfigurationContext context)
        {
            context.ContainerBuilder.AddEntityFrameworkCoreMySql();
            context.ContainerBuilder.AddDbContext<UserDatabaseDbContext>();
        }
    }
}