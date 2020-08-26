using OpenMod.API.Users;
using OpenMod.Core.Eventing;
using OpenMod.Core.Users.Events;

namespace OpenMod.Unturned.Users.Events
{
    public abstract class UnturnedPendingUserEvent : Event, IUserEvent
    {
        protected UnturnedPendingUserEvent(UnturnedPendingUser user)
        {
            User = user;
        }

        public UnturnedPendingUser User { get; }

        IUser IUserEvent.User => User;
    }
}