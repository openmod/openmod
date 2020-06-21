using Autofac;

namespace OpenMod.API.Plugins
{
    public interface IPluginContainerConfigurator 
    {
        void ConfigureContainer(ContainerBuilder containerBuilder);
    }
}