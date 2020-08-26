using OpenMod.API.Eventing;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    public interface IPlayerEvent : IEvent
    {
        IPlayer Player { get; }
    }
}