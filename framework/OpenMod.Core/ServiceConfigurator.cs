using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.API.Ioc;
using OpenMod.Core.Commands;
using OpenMod.Core.Localization;
using OpenMod.Core.Permissions;
using OpenMod.Core.Users;

namespace OpenMod.Core
{
    [UsedImplicitly]
    public class ServiceConfigurator : IServiceConfigurator
    {
        public Task ConfigureServicesAsync(IOpenModStartupContext openModStartupContext, IServiceCollection serviceCollection)
        {
            serviceCollection.Configure<PermissionCheckerOptions>(options =>
            {
                options.AddPermissionCheckProvider<DefaultPermissionCheckProvider>();
                options.AddPermissionSource<DefaultPermissionStore>();
            });

            serviceCollection.Configure<CommandStoreOptions>(options =>
            {
                var logger = openModStartupContext.LoggerFactory.CreateLogger<OpenModComponentCommandSource>();
                options.AddCommandSource(new OpenModComponentCommandSource(logger, openModStartupContext.Runtime, GetType().Assembly));
            });

            serviceCollection.Configure<UserManagerOptions>(options =>
            {
                options.AddUserProvider<OfflineUserProvider>();
            });

            serviceCollection.AddTransient<IStringLocalizerFactory, ConfigurationBasedStringLocalizerFactory>();

            return Task.CompletedTask;
        }
    }
}