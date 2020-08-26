using OpenMod.Core.Users.Events;

namespace OpenMod.Unturned.Users
{
    public class UnturnedPendingUserDisconnectedEvent : UnturnedPendingUserEvent, IUserDisconnectedEvent
    {
        public UnturnedPendingUserDisconnectedEvent(UnturnedPendingUser user) : base(user)
        {
        }
    }
}