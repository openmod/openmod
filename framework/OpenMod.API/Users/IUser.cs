using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Commands;

namespace OpenMod.API.Users
{
    /// <summary>
    ///   Represents an OpenMod user.
    /// </summary>
    public interface IUser : ICommandActor
    {
        /// <summary>
        ///    Checks if the user is currently online.
        /// </summary>
        bool IsOnline { get; }

        /// <summary>
        ///   The time the users session has begun (e.g. when the user joined the server).
        /// </summary>
        DateTime? SessionStartTime { get; }

        /// <summary>
        ///   The time the users session has stopped (e.g. when the user left the server).
        /// </summary>
        DateTime? SessionEndTime { get; }
        
        /// <summary>
        ///   Disconnects the user.
        /// </summary>
        /// <param name="reason">Disconnect reason.</param>
        Task DisconnectAsync(string reason = "");

        /// <summary>
        ///   Data for this session.
        /// </summary>
        Dictionary<string, object> SessionData { get; }

        /// <summary>
        ///    Persistent user data.
        /// </summary>
        Dictionary<string, object> PersistentData { get; }
    }
}