using OpenMod.Core.Users.Events;

namespace OpenMod.Unturned.Users.Events
{
    public class UnturnedUserFirstConnectingEvent : UnturnedUserConnectingEvent, IUserFirstConnectingEvent
    {
        public UnturnedUserFirstConnectingEvent(UnturnedPendingUser user, bool isCancelled, string rejectionReason) :
            base(user, isCancelled, rejectionReason)
        {
        }
    }
}