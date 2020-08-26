using OpenMod.Core.Users.Events;

namespace OpenMod.Unturned.Users.Events
{
    public class UnturnedUserConnectedEvent : UnturnedUserEvent, IUserConnectedEvent
    {
        public UnturnedUserConnectedEvent(UnturnedUser user) : base(user)
        {
        }
    }
}