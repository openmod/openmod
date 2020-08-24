using OpenMod.API.Users;
using OpenMod.Core.Eventing;

namespace OpenMod.Core.Users.Events
{
    public abstract class UserEvent : Event
    {
        public IUser User { get; }

        public UserEvent(IUser user)
        {
            User = user;
        }
    }
}