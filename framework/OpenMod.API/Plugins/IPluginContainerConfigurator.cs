namespace OpenMod.API.Plugins
{
    public interface IPluginContainerConfigurator
    {
        void ConfigureContainer(IPluginServiceConfigurationContext context);
    }
}