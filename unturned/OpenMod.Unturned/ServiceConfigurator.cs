extern alias JetBrainsAnnotations;
using System;
using JetBrainsAnnotations::JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Permissions;
using OpenMod.Core.Commands;
using OpenMod.Core.Permissions;
using OpenMod.Core.Users;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Configuration;
using OpenMod.Unturned.Permissions;
using OpenMod.Unturned.Players;
using OpenMod.Unturned.RocketMod;
using OpenMod.Unturned.RocketMod.Permissions;
using OpenMod.Unturned.Users;

namespace OpenMod.Unturned
{
    [UsedImplicitly]
    public class ServiceConfigurator : IServiceConfigurator
    {
        public void ConfigureServices(IOpenModServiceConfigurationContext openModStartupContext, IServiceCollection serviceCollection)
        {
            var unturnedConfiguration = new OpenModUnturnedConfiguration(openModStartupContext.Runtime.WorkingDirectory);

            serviceCollection.AddSingleton<IOpenModUnturnedConfiguration>(unturnedConfiguration);

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
                serviceCollection.AddSingleton<IRocketModComponent, RocketModComponent>();

                var permissionSystem = unturnedConfiguration.Configuration
                    .GetSection("rocketmodIntegration:permissionSystem")
                    .Get<string>();

                if (permissionSystem.Equals("RocketMod", StringComparison.OrdinalIgnoreCase))
                {
                    serviceCollection.Configure<PermissionCheckerOptions>(options =>
                    {
                        options.AddPermissionSource<RocketPermissionStore>();
                    });

                    serviceCollection.AddTransient<IPermissionRoleStore, RocketPermissionRoleStore>();
                }
            }

            serviceCollection.AddSingleton<UnturnedCommandHandler>();
        }
    }
}