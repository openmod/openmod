using OpenMod.API.Users;
using OpenMod.Core.Eventing;
using OpenMod.Core.Users.Events;

namespace OpenMod.Unturned.Users.Events
{
    public abstract class UnturnedUserEvent : Event, IUserEvent
    {
        protected UnturnedUserEvent(UnturnedUser user)
        {
            User = user;
        }

        public UnturnedUser User { get; }

        IUser IUserEvent.User => User;
    }
}