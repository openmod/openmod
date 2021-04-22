extern alias JetBrainsAnnotations;
using Autofac;
using JetBrainsAnnotations::JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Permissions;
using OpenMod.Core.Commands;
using OpenMod.Core.Permissions;
using OpenMod.Core.Users;
using OpenMod.Extensions.Economy.Abstractions;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Configuration;
using OpenMod.Unturned.Locations;
using OpenMod.Unturned.Permissions;
using OpenMod.Unturned.Players;
using OpenMod.Unturned.RocketMod;
using OpenMod.Unturned.RocketMod.Economy;
using OpenMod.Unturned.RocketMod.Permissions;
using OpenMod.Unturned.Users;
using System;

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
                var logger = openModStartupContext.LoggerFactory.CreateLogger<OpenModComponentCommandSource>();
                var host = openModStartupContext.Runtime.LifetimeScope.Resolve<IOpenModHost>();
                options.AddCommandSource(new OpenModComponentCommandSource(logger, host, typeof(OpenModUnturnedHost).Assembly));
            });

            serviceCollection.Configure<UserManagerOptions>(options =>
            {
                options.AddUserProvider<UnturnedUserProvider>();
            });

            serviceCollection.Configure<CommandParameterResolverOptions>(options =>
            {
                options.AddCommandParameterResolveProvider<UnturnedPlayerCommandParameterResolveProvider>();
                options.AddCommandParameterResolveProvider<UnturnedLocationCommandParameterResolveProvider>();
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
                        options.AddPermissionCheckProvider<RocketCooldownPermissionCheckProvider>();
                    });

                    serviceCollection.AddTransient<IPermissionRoleStore, RocketPermissionRoleStore>();
                }

                var economySystem = unturnedConfiguration.Configuration
                    .GetSection("rocketmodIntegration:economySystem")
                    .Get<string>();

                if (economySystem.Equals("RocketMod_Uconomy", StringComparison.OrdinalIgnoreCase))
                {
                    if (UconomyIntegration.IsUconomyInstalled())
                    {
                        serviceCollection.AddSingleton<IEconomyProvider, UconomyEconomyProvider>();
                    }
                    else
                    {
                        var logger = openModStartupContext.LoggerFactory.CreateLogger<RocketModIntegration>();
                        logger.LogWarning("Economy system was set to RocketMod_Uconomy but Uconomy is not installed. Defaulting to Separate");
                    }
                }
            }

            serviceCollection.AddSingleton<UnturnedCommandHandler>();
        }
    }
}