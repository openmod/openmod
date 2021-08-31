using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.Extensions.Games.Abstractions;

namespace OpenMod.Rust
{
    public class ServiceConfigurator : IServiceConfigurator
    {
        public void ConfigureServices(IOpenModServiceConfigurationContext openModStartupContext, IServiceCollection serviceCollection)
        {
            var instance = serviceCollection.FirstOrDefault(x => typeof(IHostInformation).IsAssignableFrom(x.ServiceType))?.ImplementationInstance;
            if (instance is IGameHostInformation)
            {
                serviceCollection.AddSingleton(typeof(IGameHostInformation), instance);
            }
            else
            {
                serviceCollection.AddSingleton<IGameHostInformation, RustHostInformation>();
            }

            // serviceCollection.Configure<CommandStoreOptions>(options =>
            // {
            //     options.AddCommandSource<RustCommandSource>();
            // });

            // serviceCollection.Configure<UserManagerOptions>(options =>
            // {
            //     options.AddUserProvider<RustUserProvider>();
            // });

            // serviceCollection.AddSingleton<RustCommandHandler>();
        }
    }
}