using OpenMod.API.Users;

namespace OpenMod.Core.Users.Events
{
    public class UserFirstConnectEvent : UserEvent
    {
        public UserFirstConnectEvent(IUser user) : base(user)
        {
        }
    }
}