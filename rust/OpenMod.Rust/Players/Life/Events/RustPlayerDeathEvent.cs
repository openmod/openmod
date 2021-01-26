using System.Numerics;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Life.Events
{
    public class RustPlayerDeathEvent : RustPlayerEvent, IPlayerDeathEvent
    {
        public RustPlayerDeathEvent(RustPlayer player, HitInfo? hitInfo) : base(player)
        {
            DeathPosition = player.Transform.Position;
            HitInfo = hitInfo;
        }

        public Vector3 DeathPosition { get; }
        
        public HitInfo? HitInfo { get; }
    }
}