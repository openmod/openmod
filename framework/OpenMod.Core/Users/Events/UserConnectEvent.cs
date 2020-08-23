using System;
using System.Threading.Tasks;
using OpenMod.API.Eventing;
using OpenMod.API.Users;

namespace OpenMod.Core.Users.Events
{
    public class UserConnectEvent : UserEvent, ICancellableEvent
    {
        public bool IsCancelled { get; set; }
        public string RejectionReason { get; private set; }

        public UserConnectEvent(IUser user) : base(user)
        {
        }

        public virtual Task RejectAsync(string reason)
        {
            RejectionReason = reason ?? throw new ArgumentNullException(nameof(reason));
            return Task.CompletedTask;
        }
    }
}