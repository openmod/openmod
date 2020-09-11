using OpenMod.API.Eventing;
using OpenMod.Unturned.Events;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Inventory.Events
{
    public class UnturnedPlayerDroppedItemEvent : UnturnedPlayerEvent, ICancellableEvent
    {
        public Item Item { get; }

        public bool IsCancelled { get; set; }

        public UnturnedPlayerDroppedItemEvent(UnturnedPlayer player, Item item) : base(player)
        {
            Item = item;
        }
    }
}
