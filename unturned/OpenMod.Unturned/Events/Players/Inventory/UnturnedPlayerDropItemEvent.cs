using OpenMod.API.Eventing;
using OpenMod.Unturned.Entities;
using SDG.Unturned;

namespace OpenMod.Unturned.Events.Players.Inventory
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
