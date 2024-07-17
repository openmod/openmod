extern alias JetBrainsAnnotations;
using Autofac;
using JetBrainsAnnotations::JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
                var logger = openModStartupContext.LoggerFactory.CreateLogger<RocketModIntegration>();
                logger.LogDebug("Rocketmod is installed");

                serviceCollection.AddSingleton<IRocketModComponent, RocketModComponent>();

                var permissionSystem = unturnedConfiguration.Configuration
                    .GetSection("rocketmodIntegration:permissionSystem")
                    .Get<string>() ?? string.Empty;

                if (permissionSystem.Equals("RocketMod", StringComparison.OrdinalIgnoreCase))
                {
                    logger.LogDebug("Using Rocketmod permissions system");
                    serviceCollection.Configure<PermissionCheckerOptions>(options =>
                    {
                        options.RemovePermissionSource<DefaultPermissionStore>();
                        options.AddPermissionSource<RocketPermissionStore>();
                        options.AddPermissionCheckProvider<RocketCooldownPermissionCheckProvider>();
                    });


                    for (int i = 0; i < serviceCollection.Count; i++)
                    {
                        var serviceDescriptor = serviceCollection[i];
                        if (serviceDescriptor.ServiceType != typeof(IPermissionRoleStore))
                        {
                            continue;
                        }

                        if (serviceDescriptor.ImplementationType != typeof(DefaultPermissionRoleStore))
                        {
                            continue;
                        }

                        serviceCollection.RemoveAt(i);
                    }

                    serviceCollection.AddTransient<IPermissionRoleStore, RocketPermissionRoleStore>();
                }
                else
                {
                    logger.LogDebug("Using OpenMod permissions system");
                }

                var economySystem = unturnedConfiguration.Configuration
                    .GetSection("rocketmodIntegration:economySystem")
                    .Get<string>() ?? string.Empty;

                if (economySystem.Equals("RocketMod_Uconomy", StringComparison.OrdinalIgnoreCase))
                {
                    if (UconomyIntegration.IsUconomyInstalled())
                    {
                        logger.LogDebug("Using Rocketmod uconomy system");
                        serviceCollection.AddSingleton<IEconomyProvider, UconomyEconomyProvider>();
                    }
                    else
                    {
                        logger.LogWarning(
                            "Economy system was set to RocketMod_Uconomy but Uconomy is not installed. Defaulting to Separate");
                    }
                }
                else
                {
                    logger.LogDebug("Using OpenMod economy system");
                }
            }

            serviceCollection.AddSingleton<UnturnedCommandHandler>();
        }
    }
}