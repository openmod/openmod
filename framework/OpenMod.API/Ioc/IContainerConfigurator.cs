using Autofac;

namespace OpenMod.API.Ioc
{
    /// <summary>
    /// Configurator for the OpenMod IoC container.
    /// </summary>
    public interface IContainerConfigurator
    {
        /// <summary>
        /// Called when the OpenMod root IoC container gets built.
        /// </summary>
        /// <param name="openModStartupContext">The startup context.</param>
        /// <param name="containerBuilder">The container builder.</param>
        void ConfigureContainer(IOpenModServiceConfigurationContext openModStartupContext, ContainerBuilder containerBuilder);
    }
}