using Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace OpenMod.API.Ioc
{
    public interface IContainerConfigurator
    {
        void ConfigureContainer(IRuntime runtime, ContainerBuilder containerBuilder);
    }

    public interface IServiceConfigurator
    {
        void ConfigureServices(IRuntime runtime, IServiceCollection serviceCollection);
    }
}