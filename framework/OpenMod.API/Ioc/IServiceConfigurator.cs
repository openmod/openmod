using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace OpenMod.API.Ioc
{
    public interface IServiceConfigurator
    {
        Task ConfigureServicesAsync(IOpenModStartupContext openModStartupContext, IServiceCollection serviceCollection);
    }
}