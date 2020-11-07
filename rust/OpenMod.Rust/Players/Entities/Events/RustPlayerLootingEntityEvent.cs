using OpenMod.API.Eventing;
using OpenMod.Rust.Entities;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Entities.Events
{
    public class RustPlayerLootingEntityEvent : RustPlayerEvent, ICancellableEvent
    {
        public RustEntity Entity { get; }
        public bool IsCancelled { get; set; }

        public RustPlayerLootingEntityEvent(
            RustPlayer player,
            RustEntity entity) : base(player)
        {
            Entity = entity;
        }
    }
}