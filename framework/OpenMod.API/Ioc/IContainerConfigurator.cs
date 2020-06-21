using Autofac;

namespace OpenMod.API.Ioc
{
    public interface IContainerConfigurator
    {
        void ConfigureContainer(IOpenModStartupContext openModStartupContext, ContainerBuilder containerBuilder);
    }
}