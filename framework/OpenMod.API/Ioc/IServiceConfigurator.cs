using Microsoft.Extensions.DependencyInjection;

namespace OpenMod.API.Ioc
{
    /// <summary>
    /// Configurator for the OpenMod IoC container.
    /// </summary>
    public interface IServiceConfigurator
    {
        /// <summary>
        /// Called when the OpenMod root IoC container gets built.
        /// </summary>
        /// <param name="openModStartupContext">The startup context.</param>
        /// <param name="serviceCollection">The service collection.</param>
        void ConfigureServices(IOpenModServiceConfigurationContext openModStartupContext, IServiceCollection serviceCollection);
    }
}