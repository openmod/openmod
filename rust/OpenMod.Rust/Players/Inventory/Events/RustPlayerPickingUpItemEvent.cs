using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Inventory.Events
{
    public class RustPlayerPickingUpItemEvent : RustPlayerEvent, ICancellableEvent
    {
        public Item Item { get; }
        public int TargetPos { get; }
        public bool IsCancelled { get; set; }

        public RustPlayerPickingUpItemEvent(
            RustPlayer player,
            Item item, 
            int targetPos) : base(player)
        {
            Item = item;
            TargetPos = targetPos;
        }
    }
}