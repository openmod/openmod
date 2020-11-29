using System.Numerics;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Life.Events
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