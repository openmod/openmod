using Cysharp.Threading.Tasks;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Extensions.Games.Abstractions.Items;
using OpenMod.UnityEngine.Extensions;
using System;
using System.Numerics;
using System.Threading.Tasks;
using UnityQuaternion = UnityEngine.Quaternion;
using UnityVector3 = UnityEngine.Vector3;

namespace OpenMod.Rust.Items
{
    [ServiceImplementation(Priority = Priority.Lowest)]
    public class RustItemSpawner : IItemSpawner
    {
        public Task<IItemInstance?> GiveItemAsync(IInventory inventory, string itemId, IItemState? state = null)
        {
            ValidateState(state);

            async UniTask<IItemInstance?> GiveItemTask()
            {
                await UniTask.SwitchToMainThread();

                if (inventory is RustPlayerInventory playerInventory)
                {
                    var item = await CreateItem(itemId, state);
                    if (item == null)
                    {
                        return null;
                    }

                    playerInventory.GiveItem(item.Item);
                    return new RustInventoryItem(item);
                }

                throw new NotSupportedException($"Inventory type not supported: {inventory.GetType().FullName}");
            }

            return GiveItemTask().AsTask();
        }

        public Task<IItemDrop?> SpawnItemAsync(Vector3 position, string itemId, IItemState? state = null)
        {
            ValidateState(state);

            async UniTask<IItemDrop?> SpawnItemTask()
            {
                await UniTask.SwitchToMainThread();

                var item = await CreateItem(itemId, state);
                if (item == null)
                {
                    return null;
                }

                var droppedItem = item.Item
                    .Drop(position.ToUnityVector(), UnityVector3.zero, UnityQuaternion.identity) as DroppedItem;

                return droppedItem == null ? null : new RustItemDrop(droppedItem);
            }

            return SpawnItemTask().AsTask();
        }

        private void ValidateState(IItemState? state)
        {
            if (state is NullItemState or null)
            {
                return;
            }

            if (state.ItemAmount < 1)
            {
                throw new ArgumentException($"Item amount cannot be less than 1.", nameof(state));
            }

            if (state.ItemAmount > int.MaxValue)
            {
                throw new ArgumentException($"Item amount cannot be more than {int.MaxValue}.", nameof(state));
            }
        }

        private async UniTask<RustItem?> CreateItem(string itemId, IItemState? state)
        {
            if (!int.TryParse(itemId, out var parsedItemId))
            {
                throw new ArgumentException($"Invalid Item ID: {itemId}", nameof(itemId));
            }

            await UniTask.SwitchToMainThread();

            var item = ItemManager.CreateByItemID(parsedItemId);
            if (item == null)
            {
                return null;
            }

            var rustItem = new RustItem(item);
            if (state != null && !(state is NullItemState))
            {
                await rustItem.SetItemAmountAsync(state.ItemAmount);
                await rustItem.SetItemQualityAsync(state.ItemQuality);
                await rustItem.SetItemDurabilityAsync(state.ItemDurability);
            }

            return rustItem;
        }
    }
}
