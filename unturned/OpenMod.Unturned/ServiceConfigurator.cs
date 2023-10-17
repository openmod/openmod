extern alias JetBrainsAnnotations;
using Autofac;
using JetBrainsAnnotations::JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Permissions;
using OpenMod.Core.Commands;
using OpenMod.Core.Permissions;
using OpenMod.Core.Users;
using OpenMod.Extensions.Economy.Abstractions;
using OpenMod.Extensions.Games.Abstractions;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Configuration;
using OpenMod.Unturned.Items;
using OpenMod.Unturned.Locations;
using OpenMod.Unturned.Permissions;
using OpenMod.Unturned.Players;
using OpenMod.Unturned.RocketMod;
using OpenMod.Unturned.RocketMod.Economy;
using OpenMod.Unturned.RocketMod.Permissions;
using OpenMod.Unturned.Steam;
using OpenMod.Unturned.Users;
using OpenMod.Unturned.Vehicles;
using System;

namespace OpenMod.Unturned
{
    [UsedImplicitly]
    public class ServiceConfigurator : IServiceConfigurator
    {
        public void ConfigureServices(IOpenModServiceConfigurationContext openModStartupContext,
            IServiceCollection serviceCollection)
        {
            var unturnedConfiguration =
                new OpenModUnturnedConfiguration((HostBuilderContext)openModStartupContext.DataStore["HostBuilderContext"]);

            serviceCollection.AddSingleton<IOpenModUnturnedConfiguration>(unturnedConfiguration);

            var instance = openModStartupContext.Runtime.HostInformation;
            if (instance is IGameHostInformation gameHostInformation)
            {
                serviceCollection.AddSingleton(typeof(IGameHostInformation), gameHostInformation);
            }
            else
            {
                serviceCollection.AddSingleton<IGameHostInformation, UnturnedHostInformation>();
            }

            serviceCollection.Configure<PermissionCheckerOptions>(options =>
            {
                options.AddPermissionCheckProvider<UnturnedAdminPermissionCheckProvider>();
            });

            serviceCollection.Configure<CommandStoreOptions>(options =>
            {
                options.AddCommandSource<UnturnedCommandSource>();
                var logger = openModStartupContext.LoggerFactory.CreateLogger<OpenModComponentCommandSource>();
                var host = openModStartupContext.Runtime.LifetimeScope.Resolve<IOpenModHost>();
                options.AddCommandSource(
                    new OpenModComponentCommandSource(logger, host, typeof(OpenModUnturnedHost).Assembly));
            });

            serviceCollection.Configure<UserManagerOptions>(options =>
            {
                options.AddUserProvider<UnturnedUserProvider>();
            });

            serviceCollection.Configure<CommandParameterResolverOptions>(options =>
            {
                options.AddCommandParameterResolveProvider<UnturnedPlayerCommandParameterResolveProvider>();
                options.AddCommandParameterResolveProvider<UnturnedLocationCommandParameterResolveProvider>();
                options.AddCommandParameterResolveProvider<CSteamIDCommandParameterResolveProvider>();
                options.AddCommandParameterResolveProvider<UnturnedItemAssetCommandParameterResolveProvider>();
                options.AddCommandParameterResolveProvider<UnturnedVehicleAssetCommandParameterResolveProvider>();
            });

            if (RocketModIntegration.IsRocketModInstalled())
            {
                serviceCollection.AddSingleton<IRocketModComponent, RocketModComponent>();

                var permissionSystem = unturnedConfiguration.Configuration
                    .GetSection("rocketmodIntegration:permissionSystem")
                    .Get<string>() ?? string.Empty;

                if (permissionSystem.Equals("RocketMod", StringComparison.OrdinalIgnoreCase))
                {
                    serviceCollection.Configure<PermissionCheckerOptions>(options =>
                    {
                        options.RemovePermissionSource<DefaultPermissionStore>();
                        options.AddPermissionSource<RocketPermissionStore>();
                        options.AddPermissionCheckProvider<RocketCooldownPermissionCheckProvider>();
                    });

                    serviceCollection.AddTransient<IPermissionRoleStore, RocketPermissionRoleStore>();
                }

                var economySystem = unturnedConfiguration.Configuration
                    .GetSection("rocketmodIntegration:economySystem")
                    .Get<string>() ?? string.Empty;

                if (economySystem.Equals("RocketMod_Uconomy", StringComparison.OrdinalIgnoreCase))
                {
                    if (UconomyIntegration.IsUconomyInstalled())
                    {
                        serviceCollection.AddSingleton<IEconomyProvider, UconomyEconomyProvider>();
                    }
                    else
                    {
                        var logger = openModStartupContext.LoggerFactory.CreateLogger<RocketModIntegration>();
                        logger.LogWarning(
                            "Economy system was set to RocketMod_Uconomy but Uconomy is not installed. Defaulting to Separate");
                    }
                }
            }

            serviceCollection.AddSingleton<UnturnedCommandHandler>();
        }
    }
}