using Autofac;

namespace OpenMod.API.Ioc
{
    public interface IContainerConfigurator
    {
        void ConfigureContainer(IOpenModServiceConfigurationContext openModStartupContext, ContainerBuilder containerBuilder);
    }
}