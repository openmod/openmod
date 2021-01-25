using Microsoft.Extensions.Configuration;

namespace OpenMod.API.Ioc
{
    /// <summary>
    /// Configurator for the OpenMod configuration.
    /// </summary>
    public interface IConfigurationConfigurator
    {
        /// <summary>
        /// Called when the OpenMod configuration gets built.
        /// </summary>
        void ConfigureConfiguration(IOpenModServiceConfigurationContext openModStartupContext, IConfigurationBuilder configurationBuilder);
    }
}