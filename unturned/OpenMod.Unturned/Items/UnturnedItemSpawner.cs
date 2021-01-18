using Cysharp.Threading.Tasks;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Extensions.Games.Abstractions.Items;
using OpenMod.UnityEngine.Extensions;
using SDG.Unturned;
using System;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UVector3 = UnityEngine.Vector3;
using Vector3 = System.Numerics.Vector3;

namespace OpenMod.Unturned.Items
{
    [ServiceImplementation(Priority = Priority.Lowest)]
    public class UnturnedItemSpawner : IItemSpawner
    {
        private static readonly FieldInfo s_InstanceCountField;

        static UnturnedItemSpawner()
        {
            s_InstanceCountField = typeof(ItemManager).GetField("instanceCount", BindingFlags.Static | BindingFlags.NonPublic);
        }

        public Task<IItemObject> GiveItemAsync(IInventory inventory, string itemId, IItemState state = null)
        {
            async UniTask<IItemObject> GiveItemTask()
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

                    return new UnturnedInventoryItem(itemJar, new UnturnedItem(item));
                }

                throw new NotSupportedException($"Inventory type not supported: {inventory.GetType().FullName}");
            }

            return GiveItemTask().AsTask();
        }

        private bool TryAddItem(Item item, UnturnedPlayerInventory playerInventory, out ItemJar itemJar)
        {
            var inventory = playerInventory.Inventory;
            var player = playerInventory.Player;

            itemJar = null;
            if (item == null)
            {
                return false;
            }

            ItemAsset itemAsset = (ItemAsset)Assets.find(EAssetType.ITEM, item.id);
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

        public Task<IItemDrop> SpawnItemAsync(Vector3 position, string itemId, IItemState state = null)
        {
            async UniTask<IItemDrop> SpawnItemTask()
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

        // similar to ItemManager.dropItem but with some useful return values
        private UnturnedItemDrop DropItem(Item item, Vector3 position)
        {
            var point = position.ToUnityVector();

            if (!Regions.tryGetCoordinate(point, out var x, out var y))
            {
                return null;
            }

            if (point.y > 0.0)
            {
                Physics.Raycast(point + UVector3.up, UVector3.down, out var hitInfo,
                    Mathf.Min(point.y + 1f, Level.HEIGHT), RayMasks.BLOCK_ITEM);
                if (hitInfo.collider != null)
                {
                    point.y = hitInfo.point.y;
                }
            }

            bool shouldAllow = true;
            ItemManager.onServerSpawningItemDrop?.Invoke(item, ref point, ref shouldAllow);
            if (!shouldAllow)
            {
                return null;
            }

            EffectManager.sendEffect(6, EffectManager.SMALL, point);
            var nextInstanceId = (uint)s_InstanceCountField.GetValue(null) + 1;
            s_InstanceCountField.SetValue(null, nextInstanceId);

            var itemData = new ItemData(item, nextInstanceId, point, newDropped: true);

            var region = ItemManager.regions[x, y];

            region.items.Add(itemData);
            ItemManager.instance.channel.send("tellItem",
                ESteamCall.CLIENTS, x, y, ItemManager.ITEM_REGIONS,
                ESteamPacket.UPDATE_RELIABLE_BUFFER, x, y, item.id,
                item.amount, item.quality, item.state, point, itemData.instanceID);

            return new UnturnedItemDrop(region, itemData);
        }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private void ValidateState(IItemState state)
        {
            if (state is NullItemState)
            {
                return;
            }

            if (state != null && state.ItemAmount < 1)
            {
                throw new ArgumentException($"Item amount can not be less than 1", nameof(state));
            }

            if (state != null && state.ItemAmount > byte.MaxValue)
            {
                throw new ArgumentException($"Item amount can not be more than {byte.MaxValue}", nameof(state));
            }
        }

        private Item CreateItem(ushort id, IItemState state)
        {
            var itemAsset = (ItemAsset)Assets.find(EAssetType.ITEM, id);
            if (itemAsset == null || itemAsset.isPro)
            {
                return null;
            }

            Item item = new Item(itemAsset.id, EItemOrigin.WORLD);
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