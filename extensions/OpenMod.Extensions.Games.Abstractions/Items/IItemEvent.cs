using OpenMod.API.Eventing;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    /// <summary>
    /// The base interface used for all item related events.
    /// </summary>
    public interface IItemEvent : IEvent
    {
        /// <summary>
        /// Gets the item related to the event.
        /// </summary>
        IItem Item { get; }
    }
}