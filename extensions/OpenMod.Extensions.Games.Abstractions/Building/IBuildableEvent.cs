using OpenMod.API.Eventing;

namespace OpenMod.Extensions.Games.Abstractions.Building
{
    /// <summary>
    /// The base interface for all buildable related events.
    /// </summary>
    public interface IBuildableEvent : IEvent
    {
        /// <value>
        /// The buildable related to the event.
        /// </value>
        IBuildable Buildable { get; }
    }
}