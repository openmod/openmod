using OpenMod.API.Eventing;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    public interface IPlayerChatEvent : IPlayerEvent, ICancellableEvent
    {
        string Message { get; }
    }
}