using OpenMod.API.Users;
using OpenMod.Core.Eventing;
using OpenMod.Core.Users.Events;

namespace OpenMod.Unturned.Users.Events
{
    /// <summary>
    /// The base event for all pending Unturned user related events.
    /// </summary>
    public abstract class UnturnedPendingUserEvent : Event, IUserEvent
    {
        protected UnturnedPendingUserEvent(UnturnedPendingUser user)
        {
            User = user;
        }

        /// <summary>
        /// Gets the pending Unturned user related to the event.
        /// </summary>
        public UnturnedPendingUser User { get; }

        IUser IUserEvent.User => User;
    }
}