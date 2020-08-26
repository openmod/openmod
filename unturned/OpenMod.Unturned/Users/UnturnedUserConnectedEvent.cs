using OpenMod.Core.Users.Events;

namespace OpenMod.Unturned.Users
{
    public class UnturnedUserConnectedEvent : UnturnedUserEvent, IUserConnectedEvent
    {
        public UnturnedUserConnectedEvent(UnturnedUser user) : base(user)
        {
        }
    }
}