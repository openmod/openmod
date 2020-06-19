using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.Core;
using OpenMod.Core.Console;
using OpenMod.Core.Permissions;
using OpenMod.Core.Users;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Users;

namespace OpenMod.Unturned
{
    public class ServiceConfigurator : IServiceConfigurator
    {
        public Task ConfigureServicesAsync(IOpenModStartupContext openModStartupContext, IServiceCollection serviceCollection)
        {
            serviceCollection.Configure<PermissionCheckerOptions>(options =>
            {
                options.AddPermissionCheckProvider<ConsolePermissionProvider>();
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

            return Task.CompletedTask;
        }
    }
}