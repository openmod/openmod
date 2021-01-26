using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

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
        /// <returns>The loaded plugin assemblies.</returns>
        Task<ICollection<Assembly>> LoadPluginAssembliesAsync();
    }
}