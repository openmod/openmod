using OpenMod.API.Eventing;
using OpenMod.API.Users;

namespace OpenMod.Core.Users.Events
{
    public interface IUserEvent : IEvent
    {
        IUser User { get; }
    }
}