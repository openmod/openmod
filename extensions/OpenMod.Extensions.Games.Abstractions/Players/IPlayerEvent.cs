using OpenMod.API.Eventing;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    /// <summary>
    /// The base interface for all player related events.
    /// </summary>
    public interface IPlayerEvent : IEvent
    {
        /// <summary>
        /// The player related to the event.
        /// </summary>
        IPlayer Player { get; }
    }
}