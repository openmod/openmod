using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace OpenMod.API.Plugins
{
    /// <summary>
    /// Provider for plugin assemblies.
    /// </summary>
    public interface IPluginAssembliesSource
    {
        /// <summary>
        /// Loads plugin assemblies.
        /// </summary>
        /// <returns>The loaded plugin assemblies. Cannot return null and neither can items be null.</returns>
        [NotNull]
        [ItemNotNull]
        Task<ICollection<Assembly>> LoadPluginAssembliesAsync();
    }
}