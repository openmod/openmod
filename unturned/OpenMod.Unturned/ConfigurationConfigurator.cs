using Microsoft.Extensions.Configuration;
using OpenMod.API.Ioc;

namespace OpenMod.Unturned;
internal class ConfigurationConfigurator : IConfigurationConfigurator
{
    public void ConfigureConfiguration(IOpenModServiceConfigurationContext openModStartupContext, IConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.AddYamlFile("openmod.unturned.yaml", optional: false, reloadOnChange: true);
    }
}
