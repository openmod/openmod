using OpenMod.API.Ioc;
using OpenMod.API.Plugins;

namespace OpenMod.API.Persistence
{
    /// <summary>
    /// The service used to access OpenMod's datastore.
    /// </summary>
    /// <remarks>
    /// This service cannot be used to access plugin datastores. Use <see cref="IOpenModPlugin.DataStore"/> instead.
    /// </remarks>
    [Service]
    public interface IOpenModDataStoreAccessor
    {
        /// <summary>
        /// Gets OpenMod's own datastore.
        /// </summary>
        IDataStore DataStore { get; }
    }
}