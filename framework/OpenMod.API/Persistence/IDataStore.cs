using System;
using System.Threading.Tasks;

namespace OpenMod.API.Persistence
{
    public interface IDataStore
    {
        /// <summary>
        ///    Saves data with given key.
        /// </summary>
        /// <param name="key">The key to store to.</param>
        /// <param name="data">The data to store.</param>
        Task SaveAsync<T>(string key, T data) where T: class;

        /// <summary>
        ///    Checks if the give key exists.
        /// </summary>
        /// <param name="key">The key to check.</param>
        Task<bool> ExistsAsync(string key);

        /// <summary>
        ///    Loads data from the given key.
        /// </summary>
        /// <param name="key">The keyto load from.</param>
        Task<T> LoadAsync<T>(string key) where T : class;

        IDisposable AddChangeWatcher(string key, IOpenModComponent component, Action onChange);
    }
}