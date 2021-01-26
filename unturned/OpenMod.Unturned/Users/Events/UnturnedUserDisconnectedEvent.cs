using OpenMod.Core.Users.Events;

namespace OpenMod.Unturned.Users.Events
{
    /// <summary>
    /// The event that is triggered when an Unturned user has disconnected.
    /// </summary>
    public class UnturnedUserDisconnectedEvent : UnturnedUserEvent, IUserDisconnectedEvent
    {
        public UnturnedUserDisconnectedEvent(UnturnedUser user) : base(user)
        {
        }
    }
}