using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Permissions;
using OpenMod.Core.Commands;
using OpenMod.Core.Permissions;
using OpenMod.Core.Users;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Permissions;
using OpenMod.Unturned.Players;
using OpenMod.Unturned.RocketMod;
using OpenMod.Unturned.Users;

namespace OpenMod.Unturned
{
    public class ServiceConfigurator : IServiceConfigurator
    {
        public void ConfigureServices(IOpenModServiceConfigurationContext openModStartupContext, IServiceCollection serviceCollection)
        {
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

            serviceCollection.Configure<CommandParameterResolverOptions>(options =>
            {
                options.AddCommandParameterResolveProvider<UnturnedPlayerCommandParameterResolveProvider>();
            });

            if (RocketModIntegration.IsRocketModInstalled())
            {
                // todo: check direction for permission link from config

                serviceCollection.Configure<PermissionCheckerOptions>(options =>
                {
                    options.AddPermissionSource<RocketPermissionStore>();
                });

                serviceCollection.AddTransient<IPermissionRoleStore, RocketPermissionRoleStore>();
            }

            serviceCollection.AddSingleton<UnturnedCommandHandler>();
        }
    }
}