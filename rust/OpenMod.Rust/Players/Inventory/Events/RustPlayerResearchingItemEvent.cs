using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Inventory.Events
{
    public class RustPlayerResearchingItemEvent : RustPlayerEvent, ICancellableEvent
    {
        public Item Item { get; }
        public bool IsCancelled { get; set; }

        public RustPlayerResearchingItemEvent(RustPlayer player, Item item) : base(player)
        {
            Item = item;
        }
    }
}