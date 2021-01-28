using OpenMod.Core.Users.Events;
using System;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Users.Events
{
    /// <summary>
    /// The event that is triggered when an Unturned user is connecting.
    /// </summary>
    public class UnturnedUserConnectingEvent : UnturnedPendingUserEvent, IUserConnectingEvent
    {
        public UnturnedUserConnectingEvent(UnturnedPendingUser user) : base(user)
        {
        }

        public bool IsCancelled { get; set; }

        public string? RejectionReason { get; private set; }

        public Task RejectAsync(string reason)
        {
            RejectionReason = reason ?? throw new ArgumentNullException(nameof(reason));
            return Task.CompletedTask;
        }
    }
}