using System;
using System.Threading.Tasks;
using OpenMod.API.Eventing;

namespace OpenMod.Core.Users.Events
{
    /// <summary>
    /// The event that is triggered when a user attempts to connect to the server.
    /// </summary>
    public interface IUserConnectingEvent : IUserEvent, ICancellableEvent
    {
        /// <value>
        /// The rejection reason. If not null; will reject the user. Use <see cref="RejectAsync"/> to set the value.
        /// </value>
        public string RejectionReason { get; }

        /// <summary>
        /// Rejects a players connection attempt.
        /// </summary>
        /// <param name="reason">The reason of the rejection.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="reason"/> is null.</exception>
        Task RejectAsync(string reason);
    }
}