using System.Threading.Tasks;
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
        Task SavePersistentDataAsync<T>(string key, T? data);

        /// <summary>
        /// Gets persistent data. T must be serializable.
        /// </summary>
        Task<T?> GetPersistentDataAsync<T>(string key);

        /// <summary>
        /// Gets the current user session. Returns null if the user is not online.
        /// </summary>
        IUserSession? Session { get; }

        /// <summary>
        /// Gets the user provider for the user type.
        /// </summary>
        IUserProvider? Provider { get; }
    }
}