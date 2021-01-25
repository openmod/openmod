using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OpenMod.API.Ioc;

namespace OpenMod.API.Plugins
{
    /// <summary>
    /// Service used for activating plugins.
    /// </summary>
    [OpenModInternal]
    [Service]
    public interface IPluginActivator
    {
        /// <value>
        /// Activated plugins. Cannot be null and neither items can be null.
        /// </value>
        [NotNull]
        [ItemNotNull]
        IReadOnlyCollection<IOpenModPlugin> ActivatedPlugins { get; }

        /// <summary>
        /// Tries to activate a plugin.
        /// </summary>
        /// <param name="assembly">The plugin assembly.</param>
        /// <returns><b>The plugin instance</b> if activation was successful; otherwise, <b>null</b>.</returns>
        [ItemCanBeNull]
        Task<IOpenModPlugin> TryActivatePluginAsync(Assembly assembly);
    }
}