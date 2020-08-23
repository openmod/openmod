using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenMod.API.Ioc;
using OpenMod.Core.Commands;
using OpenMod.Core.Permissions;
using OpenMod.Core.Users;
using OpenMod.UnityEngine;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Permissions;
using OpenMod.Unturned.Users;

namespace OpenMod.Unturned
{
    public class ServiceConfigurator : IServiceConfigurator
    {
        public void ConfigureServices(IOpenModServiceConfigurationContext openModStartupContext, IServiceCollection serviceCollection)
        {
            // bug: UnityEngine service configurator doesn't get called
            serviceCollection.AddSingleton<IHostLifetime, UnityHostLifetime>();
          
            serviceCollection.Configure<PermissionCheckerOptions>(options =>
            {
                options.AddPermissionCheckProvider<UnturnedAdminPermissionCheckProvider>();
            });
            
            serviceCollection.Configure<CommandStoreOptions>(options =>
            {
                options.AddCommandSource<UnturnedCommandSource>();
            });

            serviceCollection.Configure<UserManagerOptions>(options =>
            {
                options.AddUserProvider<UnturnedUserProvider>();
            });
            
            serviceCollection.AddSingleton<UnturnedCommandHandler>();
        }
    }
}