using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenMod.API.Ioc;

namespace OpenMod.UnityEngine
{
    public class ServiceConfigurator : IServiceConfigurator
    {
        public void ConfigureServices(IOpenModStartupContext openModStartupContext, IServiceCollection serviceCollection)
        {
            // bug: this method doesn't get called
            serviceCollection.AddSingleton<IHostLifetime, UnityHostLifetime>();
        }
    }
}