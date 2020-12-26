using OpenMod.API.Eventing;
using OpenMod.Rust.Items;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Inventory.Events
{
    public class RustPlayerPickingUpItemEvent : RustPlayerEvent, ICancellableEvent
    {
        public int TargetPos { get; }
        public RustItem Item { get; }
        public bool IsCancelled { get; set; }

        public RustPlayerPickingUpItemEvent(
            RustPlayer player,
            RustItem item,
            int targetPos) : base(player)
        {
            Item = item;
            TargetPos = targetPos;
        }
    }
}