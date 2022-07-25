using OpenMod.API.Plugins;
using OpenMod.EntityFrameworkCore.MySql.Extensions;
using UserDatabasePlugin.Database;

namespace UserDatabasePlugin
{
    public class ContainerConfigurator : IPluginContainerConfigurator
    {
        public void ConfigureContainer(IPluginServiceConfigurationContext context)
        {
            context.ContainerBuilder.AddMySqlDbContext<UserDatabaseDbContext>();
        }
    }
}