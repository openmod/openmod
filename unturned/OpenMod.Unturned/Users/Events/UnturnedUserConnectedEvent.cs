using OpenMod.Core.Users.Events;

namespace OpenMod.Unturned.Users.Events
{
    /// <summary>
    /// The event that is triggered when an Unturned user has connected.
    /// </summary>
    public class UnturnedUserConnectedEvent : UnturnedUserEvent, IUserConnectedEvent
    {
        public UnturnedUserConnectedEvent(UnturnedUser user) : base(user)
        {
        }
    }
}