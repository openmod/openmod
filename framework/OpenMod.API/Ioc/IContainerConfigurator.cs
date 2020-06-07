using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace OpenMod.API.Ioc
{
    public interface IContainerConfigurator
    {
        Task ConfigureContainerAsync(IOpenModStartupContext openModStartupContext, ContainerBuilder containerBuilder);
    }

    public interface IServiceConfigurator
    {
        Task ConfigureServicesAsync(IOpenModStartupContext openModStartupContext, IServiceCollection serviceCollection);
    }
}