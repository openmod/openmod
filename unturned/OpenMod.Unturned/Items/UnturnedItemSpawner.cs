using Cysharp.Threading.Tasks;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Extensions.Games.Abstractions.Items;
using OpenMod.UnityEngine.Extensions;
using SDG.Unturned;
using System;
using System.Linq;
using System.Threading.Tasks;
using UVector3 = UnityEngine.Vector3;
using Vector3 = System.Numerics.Vector3;

namespace OpenMod.Unturned.Items
{
    [ServiceImplementation(Priority = Priority.Lowest)]
    public class UnturnedItemSpawner : IItemSpawner
    {
        public Task<IItemInstance?> GiveItemAsync(IInventory inventory, string itemId, IItemState? state = null)
        {
            async UniTask<IItemInstance?> GiveItemTask()
            {
                ValidateState(state);

                await UniTask.SwitchToMainThread();

                if (!ushort.TryParse(itemId, out var parsedItemId))
                {
                    throw new ArgumentException($"Invalid Item ID: {itemId}", nameof(itemId));
                }

                if (inventory is UnturnedPlayerInventory playerInventory)
                {
                    var item = CreateItem(parsedItemId, state);
                    if (item == null)
                    {
                        return null;
                    }

                    if (!TryAddItem(item, playerInventory, out var itemJar))
                    {
                        return DropItem(item, playerInventory.Player.transform.position.ToSystemVector());
                    }

                    return new UnturnedInventoryItem(playerInventory, itemJar!);
                }

                throw new NotSupportedException($"Inventory type not supported: {inventory.GetType().FullName}");
            }

            return GiveItemTask().AsTask();
        }

        private bool TryAddItem(Item? item, UnturnedPlayerInventory playerInventory, out ItemJar? itemJar)
        {
            var inventory = playerInventory.Inventory;
            var player = playerInventory.Player;

            itemJar = null;
            if (item == null)
            {
                return false;
            }

            var itemAsset = (ItemAsset?)Assets.find(EAssetType.ITEM, item.id);
            if (itemAsset == null || itemAsset.isPro)
            {
                return false;
            }

            if (itemAsset.canPlayerEquip)
            {
                if (itemAsset.slot.canEquipAsSecondary() && TryAddItemEquip(playerInventory, item, 1 /* secondary slot */))
                {
                    itemJar = playerInventory.Inventory.items[1].getItem(index: 0);
                    return true;
                }
                if (itemAsset.slot.canEquipAsPrimary() && TryAddItemEquip(playerInventory, item, 0 /* primary slot */))
                {
                    itemJar = playerInventory.Inventory.items[0].getItem(index: 0);
                    return true;
                }
            }
            
            if (player.clothing.hat == 0 && itemAsset.type == EItemType.HAT)
            {
                player.clothing.askWearHat(item.id, item.quality, item.state, true);
                return true;
            }
            if (player.clothing.shirt == 0 && itemAsset.type == EItemType.SHIRT)
            {
                player.clothing.askWearShirt(item.id, item.quality, item.state, true);
                return true;
            }
            if (player.clothing.pants == 0 && itemAsset.type == EItemType.PANTS)
            {
                player.clothing.askWearPants(item.id, item.quality, item.state, true);
                return true;
            }
            if (player.clothing.backpack == 0 && itemAsset.type == EItemType.BACKPACK)
            {
                player.clothing.askWearBackpack(item.id, item.quality, item.state, true);
                return true;
            }
            if (player.clothing.vest == 0 && itemAsset.type == EItemType.VEST)
            {
                player.clothing.askWearVest(item.id, item.quality, item.state, true);
                return true;
            }
            if (player.clothing.mask == 0 && itemAsset.type == EItemType.MASK)
            {
                player.clothing.askWearMask(item.id, item.quality, item.state, true);
                return true;
            }
            if (player.clothing.glasses == 0 && itemAsset.type == EItemType.GLASSES)
            {
                player.clothing.askWearGlasses(item.id, item.quality, item.state, true);
                return true;
            }

            for (var page = PlayerInventory.SLOTS; page < PlayerInventory.PAGES - 2; page++)
            {
                if (!inventory.items[page].tryAddItem(item))
                {
                    continue;
                }

                itemJar = inventory.items[page].getItem((byte)(inventory.items[page].getItemCount() - 1));
                if (!player.equipment.isSelected && itemAsset.slot.canEquipInPage(page) && itemAsset.canPlayerEquip)
                {
                    player.equipment.tryEquip(page, itemJar.x, itemJar.y);
                }

                return true;
            }

            return false;
        }

        private bool TryAddItemEquip(UnturnedPlayerInventory playerInventory, Item item, byte page)
        {
            var inventory = playerInventory.Inventory;
            var player = playerInventory.Player;

            if (!inventory.items[page].tryAddItem(item))
            {
                return false;
            }

            player.equipment.sendSlot(page);
            if (!player.equipment.isSelected)
            {
                player.equipment.tryEquip(page, 0, 0);
            }
            return true;
        }

        public Task<IItemDrop?> SpawnItemAsync(Vector3 position, string itemId, IItemState? state = null)
        {
            async UniTask<IItemDrop?> SpawnItemTask()
            {
                ValidateState(state);

                await UniTask.SwitchToMainThread();

                if (!ushort.TryParse(itemId, out var parsedItemId))
                {
                    throw new ArgumentException($"Invalid Item ID: {itemId}", nameof(itemId));
                }

                var item = CreateItem(parsedItemId, state);
                if (item == null)
                {
                    return null;
                }

                return DropItem(item, position);
            }

            return SpawnItemTask().AsTask();
        }

        private UnturnedItemDrop? DropItem(Item item, Vector3 position)
        {
            var point = position.ToUnityVector();
            ItemManager.dropItem(item, point, playEffect: true, isDropped: true, wideSpread: false);

            if (!Regions.tryGetCoordinate(point, out var x, out var y))
            {
                return null;
            }

            var region = ItemManager.regions[x, y];
            var itemData = region.items.FirstOrDefault(d => d.item == item);

            return itemData == null ? null : new UnturnedItemDrop(x, y, itemData);
        }

        private void ValidateState(IItemState? state)
        {
            if (state is NullItemState or null)
            {
                return;
            }

            if (state != null && state.ItemAmount < 1)
            {
                throw new ArgumentException($"Item amount cannot be less than 1.", nameof(state));
            }

            if (state != null && state.ItemAmount > byte.MaxValue)
            {
                throw new ArgumentException($"Item amount cannot be more than {byte.MaxValue}.", nameof(state));
            }
        }

        private Item? CreateItem(ushort id, IItemState? state)
        {
            var itemAsset = (ItemAsset?)Assets.find(EAssetType.ITEM, id);
            if (itemAsset == null || itemAsset.isPro)
            {
                return null;
            }

            var item = new Item(itemAsset.id, EItemOrigin.WORLD);
            if (state != null && !(state is NullItemState))
            {
                item.state = state.StateData ?? itemAsset.getState(EItemOrigin.WORLD); /* item.state must not be null */
                item.amount = (byte)state.ItemAmount;
                item.quality = (byte)state.ItemQuality;
                item.durability = (byte)state.ItemDurability;
            }

            return item;
        }
    }
}
