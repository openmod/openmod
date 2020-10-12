using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Inventory.Events
{
    public class RustPlayerCraftingItemEvent : RustPlayerEvent, ICancellableEvent
    {
        public ItemBlueprint Blueprint { get; }
        public int Amount { get; }
        public bool IsCancelled { get; set; }
        
        public RustPlayerCraftingItemEvent(RustPlayer player, ItemBlueprint blueprint, int amount) : base(player)
        {
            Blueprint = blueprint;
            Amount = amount;
        }
    }
}