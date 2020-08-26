using OpenMod.Core.Users.Events;

namespace OpenMod.Unturned.Users
{
    public class UnturnedUserFirstConnectingEvent : UnturnedUserConnectingEvent, IUserFirstConnectingEvent
    {
        public UnturnedUserFirstConnectingEvent(UnturnedPendingUser user) : base(user)
        {
        }
    }
}