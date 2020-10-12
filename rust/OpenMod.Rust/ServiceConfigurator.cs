using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenMod.API.Ioc;
using OpenMod.UnityEngine;

namespace OpenMod.Rust
{
    public class ServiceConfigurator : IServiceConfigurator
    {
        public void ConfigureServices(IOpenModServiceConfigurationContext openModStartupContext, IServiceCollection serviceCollection)
        {
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