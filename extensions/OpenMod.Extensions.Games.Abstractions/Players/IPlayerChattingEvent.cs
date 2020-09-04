using OpenMod.API.Eventing;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    public interface IPlayerChattingEvent : IPlayerEvent, ICancellableEvent
    {
        string Message { get; }
    }
}