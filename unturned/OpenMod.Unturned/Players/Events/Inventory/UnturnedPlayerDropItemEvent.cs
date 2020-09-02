using OpenMod.API.Eventing;
using OpenMod.Unturned.Events;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Events.Inventory
{
    public class UnturnedPlayerDropItemEvent : UnturnedPlayerEvent, ICancellableEvent
    {
        public Item Item { get; }

        public bool IsCancelled { get; set; }

        public UnturnedPlayerDropItemEvent(UnturnedPlayer player, Item item) : base(player)
        {
            Item = item;
        }
    }
}
