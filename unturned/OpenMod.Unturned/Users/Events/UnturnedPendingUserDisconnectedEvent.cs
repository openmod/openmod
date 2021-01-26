using OpenMod.Core.Users.Events;

namespace OpenMod.Unturned.Users.Events
{
    /// <summary>
    /// The event that is triggered when a pending user has disconnected.
    /// </summary>
    public class UnturnedPendingUserDisconnectedEvent : UnturnedPendingUserEvent, IUserDisconnectedEvent
    {
        public UnturnedPendingUserDisconnectedEvent(UnturnedPendingUser user) : base(user)
        {
        }
    }
}