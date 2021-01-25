using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace OpenMod.API.Plugins
{
    /// <summary>
    /// The plugin assembly store used during OpenMod startup.
    /// </summary>
    /// <remarks>
    /// <b>This is an interface is for internal usage only and should not be used by plugins.</b>
    /// </remarks>
    [OpenModInternal]
    public interface IPluginAssemblyStore
    {
        /// <summary>
        /// Loads plugin assemblies from the given assembly source.
        /// </summary>
        /// <param name="source">The assemblies source.</param>
        /// <returns>The loaded plugin asemblies. Cannot return null and neither can items be null.</returns>
        [NotNull]
        [ItemNotNull]
        [OpenModInternal]
        Task<ICollection<Assembly>> LoadPluginAssembliesAsync(IPluginAssembliesSource source);

        /// <value>
        /// The loaded plugin assemblies. Cannot return null and neither can items be null.
        /// </value>
        [NotNull]
        [ItemNotNull]
        [OpenModInternal]
        IReadOnlyCollection<Assembly> LoadedPluginAssemblies { get; }
    }
}