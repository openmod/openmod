using Microsoft.Extensions.Configuration;

namespace OpenMod.API.Ioc
{
    public interface IConfigurationConfigurator
    {
        void ConfigureConfiguration(IOpenModServiceConfigurationContext openModStartupContext, IConfigurationBuilder configurationBuilder);
    }
}