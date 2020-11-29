using OpenMod.API.Eventing;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Life.Events
{
    public class RustPlayerRespawnedEvent : RustPlayerEvent, IPlayerSpawnedEvent, ICancellableEvent
    {
        public bool IsCancelled { get; set; }
        public RustPlayerRespawnedEvent(RustPlayer player) : base(player)
        {
        }
    }
}