using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace OpenMod.API.Ioc
{
    public interface IServiceConfigurator
    {
        void ConfigureServices(IOpenModStartupContext openModStartupContext, IServiceCollection serviceCollection);
    }
}