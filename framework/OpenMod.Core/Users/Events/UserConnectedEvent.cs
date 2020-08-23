using OpenMod.API.Users;

namespace OpenMod.Core.Users.Events
{
    public class UserConnectedEvent : UserEvent
    {
        public UserConnectedEvent(IUser user) : base(user)
        {
        }
    }
}