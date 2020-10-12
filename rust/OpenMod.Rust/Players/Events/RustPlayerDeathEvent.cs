using System.Numerics;
using OpenMod.Extensions.Games.Abstractions.Players;

namespace OpenMod.Rust.Players.Events
{
    public class RustPlayerDeathEvent : RustPlayerEvent, IPlayerDeathEvent
    {
        public RustPlayerDeathEvent(RustPlayer player) : base(player)
        {
            DeathPosition = player.Transform.Position;
        }

        public Vector3 DeathPosition { get; }
    }
}