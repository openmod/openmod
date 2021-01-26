using OpenMod.Core.Users.Events;

namespace OpenMod.Unturned.Users.Events
{
    /// <summary>
    /// The event that is triggered when an Unturned user connects for the first time.
    /// </summary>
    public class UnturnedUserFirstConnectingEvent : UnturnedUserConnectingEvent, IUserFirstConnectingEvent
    {
        public UnturnedUserFirstConnectingEvent(UnturnedPendingUser user) : base(user)
        {
        }
    }
}