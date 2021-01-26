using Autofac;
using Microsoft.Extensions.Configuration;

namespace OpenMod.API.Plugins
{
    /// <summary>
    /// Context used on plugin container configuration.
    /// </summary>
    public interface IPluginServiceConfigurationContext
    {
        /// <value>
        /// The parent lifetime scope.
        /// </value>
        ILifetimeScope ParentLifetimeScope { get; }

        /// <value>
        /// The plugin configuration.
        /// </value>
        IConfiguration Configuration { get; }

        /// <value>
        /// The container builder.
        /// </value>
        ContainerBuilder ContainerBuilder { get; }
    }
}