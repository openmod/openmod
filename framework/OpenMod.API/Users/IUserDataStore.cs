using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.API.Users
{
    /// <summary>
    /// The service for storing user data.
    /// </summary>
    [Service]
    public interface IUserDataStore
    {
        /// <summary>
        /// Gets user data.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="userType">The user actor type.</param>
        /// <returns>The user data.</returns>
        Task<UserData?> GetUserDataAsync(string userId, string userType);

        /// <summary>
        /// Gets user data for the given key.
        /// </summary>
        /// <typeparam name="T">The user data type.</typeparam>
        /// <param name="userId">The user id.</param>
        /// <param name="userType">The user actor type.</param>
        /// <param name="key">The data key.</param>
        /// <returns>The deserialized data.</returns>
        Task<T?> GetUserDataAsync<T>(string userId, string userType, string key);

        /// <summary>
        /// Sets user data for the given key.
        /// </summary>
        /// <typeparam name="T">The user data type.</typeparam>
        /// <param name="userId">The user id.</param>
        /// <param name="userType">The user actor type.</param>
        /// <param name="key">The data key.</param>
        /// <param name="value">The value</param>
        Task SetUserDataAsync<T>(string userId, string userType, string key, T? value);

        /// <summary>
        /// Gets all user data for the given user type.
        /// </summary>
        /// <param name="type">The user type.</param>
        /// <returns>All user data for the given user type.</returns>
        Task<IReadOnlyCollection<UserData>> GetUsersDataAsync(string type);

        /// <summary>
        /// Sets user data. Will replace data if it already exists.
        /// </summary>
        /// <param name="userData">The user data to set.</param>
        Task SetUserDataAsync(UserData userData);
    }
}