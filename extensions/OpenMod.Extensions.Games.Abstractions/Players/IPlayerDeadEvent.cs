using System.Numerics;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    public interface IPlayerDeadEvent : IPlayerEvent
    {
        Vector3 DeathPosition { get; }
    }
}