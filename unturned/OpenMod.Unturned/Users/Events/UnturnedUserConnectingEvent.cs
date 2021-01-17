using OpenMod.Core.Users.Events;
using System;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Users.Events
{
    public class UnturnedUserConnectingEvent : UnturnedPendingUserEvent, IUserConnectingEvent
    {
        public UnturnedUserConnectingEvent(UnturnedPendingUser user, bool isCancelled, string rejectionReason) : base(user)
        {
            IsCancelled = isCancelled;
            RejectionReason = rejectionReason;
        }

        public bool IsCancelled { get; set; }

        public string RejectionReason { get; private set; }

        public Task RejectAsync(string reason)
        {
            RejectionReason = reason ?? throw new ArgumentNullException(nameof(reason));
            return Task.CompletedTask;
        }
    }
}