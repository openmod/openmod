using OpenMod.API.Eventing;

namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    /// <summary>
    /// The base interface for all entity related events.
    /// </summary>
    public interface IEntityEvent : IEvent
    {
        /// <summary>
        /// Gets the entity related to the event.
        /// </summary>
        IEntity Entity { get; }
    }
}