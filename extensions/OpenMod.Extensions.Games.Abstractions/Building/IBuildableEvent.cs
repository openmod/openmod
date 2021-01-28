using OpenMod.API.Eventing;

namespace OpenMod.Extensions.Games.Abstractions.Building
{
    /// <summary>
    /// The base interface for all buildable related events.
    /// </summary>
    public interface IBuildableEvent : IEvent
    {
        /// <summary>
        /// Gets the buildable related to the event.
        /// </summary>
        IBuildable Buildable { get; }
    }
}