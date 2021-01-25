using OpenMod.API.Eventing;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    /// <summary>
    /// The base interface used for all item related events.
    /// </summary>
    public interface IItemEvent : IEvent
    {
        /// <value>
        /// The item related to the event.
        /// </value>
        IItem Item { get; }
    }
}