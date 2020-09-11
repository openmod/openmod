using OpenMod.API.Eventing;
using OpenMod.Unturned.Events;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Inventory.Events
{
    public class UnturnedPlayerTakingItemEvent : UnturnedPlayerEvent, ICancellableEvent
    {
        public byte RegionX { get; }

        public byte RegionY { get; }

        public uint InstanceId { get; }

        public byte InventoryX { get; }

        public byte InventoryY { get; }

        public byte InventoryRotation { get; }

        public byte InventoryPage { get; }

        public ItemData ItemData { get; }

        public bool IsCancelled { get; set; }

        public UnturnedPlayerTakingItemEvent(UnturnedPlayer player, byte regionX, byte regionY, uint instanceId, byte inventoryX, byte inventoryY, byte inventoryRotation, byte inventoryPage, ItemData itemData) : base(player)
        {
            RegionX = regionX;
            RegionY = regionY;
            InstanceId = instanceId;
            InventoryX = inventoryX;
            InventoryY = inventoryY;
            InventoryRotation = inventoryRotation;
            InventoryPage = inventoryPage;
            ItemData = itemData;
        }
    }
}
