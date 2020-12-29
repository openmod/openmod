using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Entities.Events
{
    public class RustPlayerLootedItemEvent : RustPlayerEvent
    {
        public Item Item { get; }
        
        public RustPlayerLootedItemEvent(
            RustPlayer player,
            Item item) : base(player)
        {
            Item = item;
        }
    }
}