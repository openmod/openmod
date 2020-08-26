using OpenMod.Core.Users.Events;

namespace OpenMod.Unturned.Users.Events
{
    public class UnturnedPendingUserDisconnectedEvent : UnturnedPendingUserEvent, IUserDisconnectedEvent
    {
        public UnturnedPendingUserDisconnectedEvent(UnturnedPendingUser user) : base(user)
        {
        }
    }
}