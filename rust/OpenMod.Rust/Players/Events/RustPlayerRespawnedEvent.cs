using OpenMod.API.Eventing;
using OpenMod.Extensions.Games.Abstractions.Players;

namespace OpenMod.Rust.Players.Events
{
    public class RustPlayerRespawnedEvent : RustPlayerEvent, IPlayerSpawnedEvent, ICancellableEvent
    {
        public bool IsCancelled { get; set; }
        public RustPlayerRespawnedEvent(RustPlayer player) : base(player)
        {
        }
    }
}