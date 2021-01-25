using Autofac;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;

namespace OpenMod.API.Plugins
{
    /// <summary>
    /// Context used on plugin container configuration.
    /// </summary>
    public interface IPluginServiceConfigurationContext
    {
        /// <value>
        /// The parent lifetime scope. Cannot be null. This is not the plugins lifetime scope.
        /// </value>
        [NotNull]
        ILifetimeScope ParentLifetimeScope { get; }

        /// <value>
        /// The plugin configuration. Cannot be null.
        /// </value>
        [NotNull]
        IConfiguration Configuration { get; }

        /// <value>
        /// The container builder. Cannot be null.
        /// </value>
        [NotNull]
        ContainerBuilder ContainerBuilder { get; }
    }
}