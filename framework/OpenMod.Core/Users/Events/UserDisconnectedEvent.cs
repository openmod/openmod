using OpenMod.API.Users;
using OpenMod.Core.Eventing;

namespace OpenMod.Core.Users.Events
{
    public sealed class UserDisconnectedEvent : Event
    {
        public IUser User { get; }

        public UserDisconnectedEvent(IUser user)
        {
            User = user;
        }
    }
}