using OpenMod.API.Eventing;

namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    /// <summary>
    /// The base interface for all entity related events.
    /// </summary>
    public interface IEntityEvent : IEvent
    {
        /// <value>
        /// The entity related to the event.
        /// </value>
        IEntity Entity { get; }
    }
}