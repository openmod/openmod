using OpenMod.API.Eventing;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    /// <summary>
    /// The event that is triggered before a player message is broadcasted.
    /// </summary>
    public interface IPlayerChattingEvent : IPlayerEvent, ICancellableEvent
    {
        /// <value>
        /// The chat message to be broadcasted.
        /// </value>
        string Message { get; }
    }
}