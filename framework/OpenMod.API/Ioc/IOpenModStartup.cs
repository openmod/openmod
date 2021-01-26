using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using OpenMod.API.Plugins;

namespace OpenMod.API.Ioc
{
    /// <summary>
    /// Utility class for common functions used during OpenMod startup.
    /// </summary>
    public interface IOpenModStartup
    {
        /// <summary>
        /// The service configuration context.
        /// </summary>
        IOpenModServiceConfigurationContext Context { get; }

        /// <summary>
        /// Registers services using the <c>[Service]</c> and <c>[ServiceImplementation]</c> attributes from the given assembly.
        /// Also copies the embedded resources to the given assembly directory.
        /// </summary>
        /// <param name="assembly">The assembly to register the services from.</param>
        /// <param name="assemblyDir">The directory the embedded resources get copied to.</param>
        void RegisterIocAssemblyAndCopyResources(Assembly assembly, string assemblyDir);

        /// <summary>
        /// Registers plugin assemblies.
        /// </summary>
        /// <param name="source">The plugin assemblies source.</param>
        /// <returns>The loaded plugin assemblies.</returns>
        Task<ICollection<Assembly>> RegisterPluginAssembliesAsync(IPluginAssembliesSource source);
    }
}