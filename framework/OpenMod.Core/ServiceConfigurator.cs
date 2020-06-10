using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using OpenMod.API.Ioc;
using OpenMod.Core.Commands;
using OpenMod.Core.Localization;
using OpenMod.Core.Permissions;

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

            serviceCollection.Configure<CommandExecutorOptions>(options =>
            {
                options.AddCommandSource(new OpenModComponentCommandSource(openModStartupContext.Runtime));
            });

            serviceCollection.AddTransient<IStringLocalizerFactory, ConfigurationBasedStringLocalizerFactory>();

            return Task.CompletedTask;
        }
    }
}