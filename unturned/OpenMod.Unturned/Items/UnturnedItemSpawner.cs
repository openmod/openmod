using Cysharp.Threading.Tasks;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Extensions.Games.Abstractions.Items;
using OpenMod.UnityEngine.Extensions;
using SDG.Unturned;
using System;
using System.Linq;
using System.Threading.Tasks;
using OpenMod.Unturned.Players.Clothing;
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

                if (!ushort.TryParse(itemId, out var parsedItemId))
                {
                    throw new ArgumentException($"Invalid Item ID: {itemId}", nameof(itemId));
                }

                if (inventory is not UnturnedPlayerInventory playerInventory)
                    throw new NotSupportedException($"Inventory type not supported: {inventory.GetType().FullName}");

                var item = CreateItem(parsedItemId, state, out var itemAsset);
                if (item == null || itemAsset == null)
                {
                    return null;
                }

                await UniTask.SwitchToMainThread();

                if (!TryAddItem(itemAsset, item, playerInventory, out var itemJar))
                {
                    return DropItem(item, playerInventory.Player.transform.position.ToSystemVector());
                }

                if (itemJar != null)
                {
                    return new UnturnedInventoryItem(playerInventory, itemJar);

                }

                // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
                var clothType = itemAsset.type switch
                {
                    EItemType.BACKPACK => ClothingType.Backpack,
                    EItemType.GLASSES => ClothingType.Glasses,
                    EItemType.HAT => ClothingType.Hat,
                    EItemType.MASK => ClothingType.Mask,
                    EItemType.PANTS => ClothingType.Pants,
                    EItemType.SHIRT => ClothingType.Shirt,
                    EItemType.VEST => ClothingType.Vest,
                    _ => throw new ArgumentOutOfRangeException(nameof(itemAsset.type), itemAsset.type, "Type is not a cloth")
                };

                return new UnturnedClothingItem(item, playerInventory.Player.clothing, clothType);
            }

            return GiveItemTask().AsTask();
        }

        private bool TryAddItem(ItemAsset itemAsset, Item? item, UnturnedPlayerInventory playerInventory, out ItemJar? itemJar)
        {
            var inventory = playerInventory.Inventory;
            var player = playerInventory.Player;

            itemJar = null;
            if (item == null)
            {
                return false;
            }

            if (itemAsset.isPro)
            {
                return false;
            }

            if (itemAsset.canPlayerEquip)
            {
                if (itemAsset.slot.canEquipAsSecondary() && TryAddItemEquip(playerInventory, item, page: 1 /* secondary slot */))
                {
                    itemJar = playerInventory.Inventory.items[1].getItem(index: 0);
                    return true;
                }
                if (itemAsset.slot.canEquipAsPrimary() && TryAddItemEquip(playerInventory, item, page: 0 /* primary slot */))
                {
                    itemJar = playerInventory.Inventory.items[0].getItem(index: 0);
                    return true;
                }
            }

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (itemAsset.type)
            {
                case EItemType.BACKPACK:
                    if (player.clothing.backpack != 0)
                        break;

                    player.clothing.askWearBackpack(item.id, item.quality, item.state, playEffect: true);
                    return true;

                case EItemType.GLASSES:
                    if (player.clothing.glasses != 0)
                        break;

                    player.clothing.askWearGlasses(item.id, item.quality, item.state, playEffect: true);
                    return true;

                case EItemType.HAT:
                    if (player.clothing.hat != 0)
                        break;

                    player.clothing.askWearHat(item.id, item.quality, item.state, playEffect: true);
                    return true;

               case EItemType.MASK:
                    if (player.clothing.mask != 0)
                        break;

                    player.clothing.askWearMask(item.id, item.quality, item.state, playEffect: true);
                    return true;

                case EItemType.PANTS:
                    if (player.clothing.pants != 0)
                        break;

                    player.clothing.askWearPants(item.id, item.quality, item.state, playEffect: true);
                    return true;

                case EItemType.SHIRT:
                    if (player.clothing.shirt != 0)
                        break;

                    player.clothing.askWearShirt(item.id, item.quality, item.state, playEffect: true);
                    return true;

                case EItemType.VEST:
                    if (player.clothing.vest != 0)
                        break;

                    player.clothing.askWearVest(item.id, item.quality, item.state, playEffect: true);
                    return true;
            }

            for (var page = PlayerInventory.SLOTS; page < PlayerInventory.PAGES - 2; page++)
            {
                if (!inventory.items[page].tryAddItem(item))
                {
                    continue;
                }

                itemJar = inventory.items[page].getItem((byte)(inventory.items[page].getItemCount() - 1));
                if (!player.equipment.HasValidUseable && itemAsset.slot.canEquipInPage(page) && itemAsset.canPlayerEquip)
                {
                    player.equipment.ServerEquip(page, itemJar.x, itemJar.y);
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
            if (!player.equipment.HasValidUseable)
            {
                player.equipment.ServerEquip(page, 0, 0);
            }
            return true;
        }

        public Task<IItemDrop?> SpawnItemAsync(Vector3 position, string itemId, IItemState? state = null)
        {
            async UniTask<IItemDrop?> SpawnItemTask()
            {
                ValidateState(state);

                if (!ushort.TryParse(itemId, out var parsedItemId))
                {
                    throw new ArgumentException($"Invalid Item ID: {itemId}", nameof(itemId));
                }

                var item = CreateItem(parsedItemId, state, out _);
                if (item == null)
                {
                    return null;
                }

                await UniTask.SwitchToMainThread();

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
            switch (state)
            {
                case NullItemState or null:
                    return;

                case {ItemAmount: < 1}:
                    throw new ArgumentException("Item amount cannot be less than 1.", nameof(state));

                case {ItemAmount: > byte.MaxValue}:
                    throw new ArgumentException($"Item amount cannot be more than {byte.MaxValue}.", nameof(state));
            }
        }

        private Item? CreateItem(ushort id, IItemState? state, out ItemAsset? itemAsset)
        {
            itemAsset = (ItemAsset?)Assets.find(EAssetType.ITEM, id);
            if (itemAsset == null || itemAsset.isPro)
            {
                return null;
            }

            var item = new Item(itemAsset.id, EItemOrigin.ADMIN);
            if (state is null or NullItemState)
                return item;

            item.state = state.StateData ?? itemAsset.getState(EItemOrigin.ADMIN); /* item.state must not be null */
            item.amount = (byte)state.ItemAmount;
            item.quality = (byte)state.ItemQuality;
            item.durability = (byte)state.ItemDurability;

            return item;
        }
    }
}
