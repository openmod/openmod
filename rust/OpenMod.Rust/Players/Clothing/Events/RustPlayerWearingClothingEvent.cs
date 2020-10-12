using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Clothing.Events
{
    public class RustPlayerWearingClothingEvent : RustPlayerEvent, ICancellableEvent
    {
        public Item Item { get; }
        public int Slot { get; }
        public bool IsCancelled { get; set; }
     
        public RustPlayerWearingClothingEvent(
            RustPlayer player, 
            Item item, 
            int slot) : base(player)
        {
            Item = item;
            Slot = slot;
        }
    }
}