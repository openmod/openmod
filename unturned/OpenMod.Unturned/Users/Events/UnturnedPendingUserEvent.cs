using OpenMod.API.Users;
using OpenMod.Core.Eventing;
using OpenMod.Core.Users.Events;

namespace OpenMod.Unturned.Users.Events
{
    /// <summary>
    /// The base event for all Unturned pending user related events.
    /// </summary>
    public abstract class UnturnedPendingUserEvent : Event, IUserEvent
    {
        protected UnturnedPendingUserEvent(UnturnedPendingUser user)
        {
            User = user;
        }

        /// <value>
        /// The Unturned pending user related to the event.
        /// </value>
        public UnturnedPendingUser User { get; }

        IUser IUserEvent.User => User;
    }
}