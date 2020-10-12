using OpenMod.API.Eventing;

namespace OpenMod.Rust.Players.Events
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