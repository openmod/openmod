using OpenMod.API.Eventing;

namespace OpenMod.Extensions.Games.Abstractions.Building
{
    /// <summary>
    /// The interface for any buildable transforming events.
    /// </summary>
    public interface IBuildableTransformingEvent : IBuildableEvent, ICancellableEvent
    {
    }
}