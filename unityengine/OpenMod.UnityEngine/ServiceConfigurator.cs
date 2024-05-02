using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenMod.API.Ioc;
using OpenMod.UnityEngine.Helpers;

namespace OpenMod.UnityEngine
{
    public class ServiceConfigurator : IServiceConfigurator
    {
        public void ConfigureServices(IOpenModServiceConfigurationContext openModStartupContext, IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IHostLifetime, UnityHostLifetime>();
            serviceCollection.AddHostedService<TlsHandshakeWorkaround>();
        }
    }
}