using JetBrains.Annotations;
using OpenMod.API.Ioc;

namespace OpenMod.API.Persistence
{
    /// <summary>
    /// The service used to create data stores.
    /// <seealso cref="IDataStore"/>.
    /// </summary>
    [Service]
    public interface IDataStoreFactory
    {
        /// <summary>
        /// Creates a new datastore.
        /// </summary>
        /// <param name="parameters">The data store creation parameters</param>
        /// <returns>The created data store. Cannot return null.</returns>
        [NotNull]
        IDataStore CreateDataStore(DataStoreCreationParameters parameters);
    }
}