using OpenMod.API.Eventing;
using OpenMod.Rust.Entities;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Entities.Events
{
    public class RustPlayerPickingUpEntityEvent : RustPlayerEvent, ICancellableEvent
    {
        public RustEntity Entity { get; }

        public RustPlayerPickingUpEntityEvent(
            RustPlayer player,
            RustEntity entity) : base(player)
        {
            Entity = entity;
        }

        public bool IsCancelled { get; set; }
    }
}