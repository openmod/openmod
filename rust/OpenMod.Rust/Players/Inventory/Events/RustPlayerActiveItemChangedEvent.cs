using OpenMod.Rust.Items;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Inventory.Events
{
    public class RustPlayerActiveItemChangedEvent : RustPlayerEvent
    {
        public RustItem OldItem { get; }
        public RustItem NewItem { get; }

        public RustPlayerActiveItemChangedEvent(
            RustPlayer player,
            RustItem oldItem,
            RustItem newItem) : base(player)
        {
            OldItem = oldItem;
            NewItem = newItem;
        }
    }
}