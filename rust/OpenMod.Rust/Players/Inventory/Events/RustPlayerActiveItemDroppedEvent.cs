using OpenMod.API.Eventing;
using OpenMod.Rust.Items;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Inventory.Events
{
    public class RustPlayerActiveItemDroppedEvent : RustPlayerEvent, ICancellableEvent
    {
        public RustItem Item { get; }
        public bool IsCancelled { get; set; }

        public RustPlayerActiveItemDroppedEvent(
            RustPlayer player, 
            RustItem item) : base(player)
        {
            Item = item;
        }
    }
}