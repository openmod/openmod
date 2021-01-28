using OpenMod.API.Eventing;
using OpenMod.API.Users;

namespace OpenMod.Core.Users.Events
{
    /// <summary>
    /// The base interface for all user related events.
    /// </summary>
    public interface IUserEvent : IEvent
    {
        /// <summary>
        /// Gets the user related to the event.
        /// </summary>
        IUser User { get; }
    }
}