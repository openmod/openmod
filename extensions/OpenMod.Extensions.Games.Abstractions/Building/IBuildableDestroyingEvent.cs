using OpenMod.API.Eventing;

namespace OpenMod.Extensions.Games.Abstractions.Building
{
    /// <summary>
    /// The interface for any buildable destroying events.
    /// </summary>
    public interface IBuildableDestroyingEvent : IBuildableEvent, ICancellableEvent
    {
    }
}