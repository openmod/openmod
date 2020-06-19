using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenMod.API.Users
{
    public interface IUserSession
    {
        /// <summary>
        ///   The time the users session has begun (e.g. when the user joined the server).
        /// </summary>
        DateTime? SessionStartTime { get; }

        /// <summary>
        ///   The time the users session has stopped (e.g. when the user left the server).
        /// </summary>
        DateTime? SessionEndTime { get; }

        /// <summary>
        ///   Data for this session.
        /// </summary>
        Dictionary<string, object> SessionData { get; }

        /// <summary>
        ///   Disconnects the user.
        /// </summary>
        /// <param name="reason">Disconnect reason.</param>
        Task DisconnectAsync(string reason = "");
    }
}