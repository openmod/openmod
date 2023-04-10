using Autofac;
using Microsoft.Extensions.Configuration;

namespace OpenMod.API.Plugins
{
    /// <summary>
    /// Context used on plugin container configuration.
    /// </summary>
    public interface IPluginServiceConfigurationContext
    {
        /// <summary>
        /// Gets the parent lifetime scope.
        /// </summary>
        ILifetimeScope ParentLifetimeScope { get; }

        /// <summary>
        /// Gets the plugin configuration.
        /// </summary>
        IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the container builder.
        /// </summary>
        ContainerBuilder ContainerBuilder { get; }

        /// <summary>
        /// Gets the plugin working directory.
        /// </summary>
        string WorkingDirectory { get; }
    }
}