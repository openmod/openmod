using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Core.Eventing;

namespace OpenMod.Core.Users.Events
{
    public sealed class UserConnectingEvent : Event, ICancellableEvent
    {
        public IUser User { get; }
        public bool IsCancelled { get; set; }
        public string RejectionReason { get; private set; }

        public UserConnectingEvent(IUser user)
        {
            User = user;
        }

        public void Reject(string reason)
        {
            RejectionReason = reason;
        }
    }
}