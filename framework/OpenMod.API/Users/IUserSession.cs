using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace OpenMod.API.Users
{
    /// <summary>
    /// Represents a user session.
    /// </summary>
    public interface IUserSession
    {
        /// <value>
        /// The time the session has begun. Can be null.
        /// </value>
        /// <example>
        /// The time a user has joined a server.
        /// </example>
        [CanBeNull]
        DateTime? SessionStartTime { get; }

        /// <value>
        /// The time the users session has ended. Can be null.
        /// </value>
        /// <example>
        /// The time a user has left a server.
        /// </example>
        [CanBeNull]
        DateTime? SessionEndTime { get; }

        /// <value>
        /// The user data for this session.
        /// </value>
        [NotNull]
        Dictionary<string, object> SessionData { get; }

        /// <value>
        /// The user data that is stored for process lifetime and will survive reloads.
        /// </value>
        [NotNull]
        Dictionary<string, object> InstanceData { get; }

        /// <summary>
        /// Disconnects the user.
        /// </summary>
        /// <param name="reason">The optional disconnect reason to be showed to the user.</param>
        Task DisconnectAsync(string reason = "");
    }
}