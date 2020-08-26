using OpenMod.Core.Users.Events;

namespace OpenMod.Unturned.Users
{
    public class UnturnedUserDisconnectedEvent : UnturnedUserEvent, IUserDisconnectedEvent
    {
        public UnturnedUserDisconnectedEvent(UnturnedUser user) : base(user)
        {
        }
    }
}