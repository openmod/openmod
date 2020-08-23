using OpenMod.API.Users;

namespace OpenMod.Core.Users.Events
{
    public class UserDisconnectedEvent : UserEvent
    {
        public UserDisconnectedEvent(IUser user) : base(user)
        {
        }
    }
}