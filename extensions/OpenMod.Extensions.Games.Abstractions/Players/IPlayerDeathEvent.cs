using System.Numerics;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    public interface IPlayerDeathEvent : IPlayerEvent
    {
        Vector3 DeathPosition { get; }
    }
}