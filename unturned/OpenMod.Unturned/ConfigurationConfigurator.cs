using Microsoft.Extensions.Configuration;
using OpenMod.API.Ioc;

namespace OpenMod.Unturned
{
    internal class ConfigurationConfigurator : IConfigurationConfigurator
    {
        public void ConfigureConfiguration(IOpenModServiceConfigurationContext openModStartupContext, IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.AddYamlFileEx("openmod.unturned.yaml", optional: false, reloadOnChange: true);
        }
    }
}
