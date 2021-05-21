using OpenMod.API.Eventing;

namespace OpenMod.Extensions.Games.Abstractions.Building
{
    /// <summary>
    /// The interface for any buildable deploying events.
    /// </summary>
    public interface IBuildableDeployingEvent : IEvent, ICancellableEvent
    {
        /// <summary>
        /// Gets the buildable asset that is going to be used whilst deploying.
        /// </summary>
        IBuildableAsset BuildableAsset { get; }
    }
}