using System.Threading.Tasks;
using JetBrains.Annotations;
using OpenMod.API.Commands;

namespace OpenMod.API.Users
{
    /// <summary>
    /// Represents an OpenMod user.
    /// </summary>
    public interface IUser : ICommandActor
    {
        /// <summary>
        /// Saves persistent data. T must be serializable.
        /// </summary>
        Task SavePersistentDataAsync<T>(string key, T data);

        /// <summary>
        /// Gets persistent data. T must be serializable.
        /// </summary>
        Task<T> GetPersistentDataAsync<T>(string key);

        /// <value>
        /// Represents the current user session. Can be null if the user is not online.
        /// </value>
        [CanBeNull]
        IUserSession Session { get; }

        /// <value>
        /// The user provider for this user type. Can be null.
        /// </value>
        [CanBeNull]
        IUserProvider Provider { get; }
    }
}