using OpenMod.API.Users;
using OpenMod.Core.Eventing;

namespace OpenMod.Core.Users.Events
{
    public sealed class UserConnectedEvent : Event
    {
        public IUser User { get; }

        public UserConnectedEvent(IUser user)
        {
            User = user;
        }
    }
}