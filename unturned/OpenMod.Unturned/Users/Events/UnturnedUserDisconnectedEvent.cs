using OpenMod.Core.Users.Events;

namespace OpenMod.Unturned.Users.Events
{
    public class UnturnedUserDisconnectedEvent : UnturnedUserEvent, IUserDisconnectedEvent
    {
        public UnturnedUserDisconnectedEvent(UnturnedUser user) : base(user)
        {
        }
    }
}