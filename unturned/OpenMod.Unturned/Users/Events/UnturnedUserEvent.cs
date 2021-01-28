using OpenMod.API.Users;
using OpenMod.Core.Eventing;
using OpenMod.Core.Users.Events;

namespace OpenMod.Unturned.Users.Events
{
    /// <summary>
    /// The base event for all Unturned user related events.
    /// </summary>
    public abstract class UnturnedUserEvent : Event, IUserEvent
    {
        protected UnturnedUserEvent(UnturnedUser user)
        {
            User = user;
        }

        /// <summary>
        /// Gets the Unturned user related to the event.
        /// </summary>
        public UnturnedUser User { get; }

        IUser IUserEvent.User => User;
    }
}