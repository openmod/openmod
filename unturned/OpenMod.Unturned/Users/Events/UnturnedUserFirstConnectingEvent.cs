using OpenMod.Core.Users.Events;

namespace OpenMod.Unturned.Users.Events
{
    public class UnturnedUserFirstConnectingEvent : UnturnedUserConnectingEvent, IUserFirstConnectingEvent
    {
        public UnturnedUserFirstConnectingEvent(UnturnedPendingUser user) : base(user)
        {
        }
    }
}