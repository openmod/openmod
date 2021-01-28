using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenMod.API.Users
{
    /// <summary>
    /// Represents a user session.
    /// </summary>
    public interface IUserSession
    {
        /// <summary>
        /// Gets the time the session has begun. Returns null if the session has not started.
        /// </summary>
        /// <example>
        /// The time a user has joined a server.
        /// </example>
        DateTime? SessionStartTime { get; }

        /// <summary>
        /// Gets the time the users session has ended. Return null if the session has not started or has not ended yet.
        /// </summary>
        /// <example>
        /// The time a user has left a server.
        /// </example>
        DateTime? SessionEndTime { get; }

        /// <summary>
        /// Gets the user data for this session.
        /// </summary>
        Dictionary<string, object?> SessionData { get; }

        /// <summary>
        /// Gets the user data that is stored for process lifetime and will survive reloads.
        /// </summary>
        Dictionary<string, object?> InstanceData { get; }

        /// <summary>
        /// Disconnects the user.
        /// </summary>
        /// <param name="reason">The optional disconnect reason to be showed to the user.</param>
        Task DisconnectAsync(string reason = "");
    }
}