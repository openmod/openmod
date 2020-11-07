using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Entities.Events
{
    public class RustPlayerDismountingEntityEvent : RustPlayerEvent, ICancellableEvent
    {
        public BaseMountable Mountable { get; }
        public bool IsCancelled { get; set; }

        public RustPlayerDismountingEntityEvent(RustPlayer player, BaseMountable mountable) : base(player)
        {
            Mountable = mountable;
        }
    }
}