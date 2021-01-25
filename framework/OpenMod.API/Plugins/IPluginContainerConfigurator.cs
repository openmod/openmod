namespace OpenMod.API.Plugins
{
    /// <summary>
    /// Configurator for plugin containers.
    /// </summary>
    public interface IPluginContainerConfigurator
    {
        /// <summary>
        /// Called when a plugins container gets configured.
        /// </summary>
        /// <param name="context">The configuration context.</param>
        void ConfigureContainer(IPluginServiceConfigurationContext context);
    }
}