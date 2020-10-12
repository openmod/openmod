using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Inventory.Events
{
    public class RustPlayerActiveItemChangedEvent : RustPlayerEvent
    {
        public Item OldItem { get; }
        public Item NewItem { get; }

        public RustPlayerActiveItemChangedEvent(
            RustPlayer player,
            Item oldItem,
            Item newItem) : base(player)
        {
            OldItem = oldItem;
            NewItem = newItem;
        }
    }
}