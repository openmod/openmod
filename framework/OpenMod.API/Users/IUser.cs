using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OpenMod.API.Commands;

namespace OpenMod.API.Users
{
    /// <summary>
    ///   Represents an OpenMod user.
    /// </summary>
    public interface IUser : ICommandActor
    {
        /// <summary>
        ///   Saves persistent data. T must be serializable.
        /// </summary>
        Task SavePersistentDataAsync<T>(string key, T data) where T : class;

        /// <summary>
        ///   Gets persistent data. T must be serializable.
        /// </summary>
        Task<T> GetPersistentDataAsync<T>(string key) where T : class;

        /// <summary>
        ///    Represents the current user session. <b>Can be null</b> if the user is not online.
        /// </summary>
        [CanBeNull]
        IUserSession Session { get; }
    }
}