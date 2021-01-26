using System;
using System.Threading.Tasks;

namespace OpenMod.API.Persistence
{
    /// <summary>
    /// Provides persistent storage.
    /// </summary>
    public interface IDataStore
    {
        /// <summary>
        /// Saves data with given key.
        /// </summary>
        /// <param name="key">The key to store to.</param>
        /// <param name="data">The data to store to.</param>
        Task SaveAsync<T>(string key, T? data) where T: class;

        /// <summary>
        /// Checks if the give key exists.
        /// </summary>
        /// <param name="key">The key to check.</param>
        Task<bool> ExistsAsync(string key);

        /// <summary>
        /// Loads data from the given key.
        /// </summary>
        /// <param name="key">The key to load from.</param>
        Task<T?> LoadAsync<T>(string key) where T : class;

        /// <summary>
        /// Adds a change watcher if the data has changed externally. Does not trigger for calls to <see cref="SaveAsync{T}"/>.
        /// </summary>
        /// <param name="key">The key to listen to.</param>
        /// <param name="component">The component registering the change watcher.</param>
        /// <param name="onChange">The on change callback.</param>
        /// <returns>An <see cref="IDisposable"/> that will unregister the change watcher on disposal.</returns>
        IDisposable AddChangeWatcher(string key, IOpenModComponent component, Action onChange);
    }
}