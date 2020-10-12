using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Entities.Events
{
    public class RustPlayerMountingEntityEvent : RustPlayerEvent, ICancellableEvent
    {
        public BaseMountable Mountable { get; }
        public bool IsCancelled { get; set; }

        public RustPlayerMountingEntityEvent(RustPlayer player, BaseMountable mountable) : base(player)
        {
            Mountable = mountable;
        }
    }
}