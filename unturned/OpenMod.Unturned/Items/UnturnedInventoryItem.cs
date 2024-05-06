﻿using Cysharp.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Items;
using SDG.Unturned;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Items
{
    public class UnturnedInventoryItem : IInventoryItem
    {
        public UnturnedInventoryItem(UnturnedPlayerInventory inventory, ItemJar itemJar)
        {
            Inventory = inventory;
            ItemJar = itemJar;
            Item = new UnturnedItem(itemJar.item, DestroyAsync);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public UnturnedPlayerInventory Inventory { get; }

        // ReSharper disable once MemberCanBePrivate.Global
        public ItemJar ItemJar { get; }

        IItem IItemInstance.Item
        {
            get => Item;
        }

        public UnturnedItem Item { get; }

        public Task DropAsync()
        {
            var transform = Inventory.Player.transform;
            ItemManager.dropItem(ItemJar.item,
                transform.position + transform.forward * 0.5f,
                playEffect: true, isDropped: true, wideSpread: false);

            return DestroyAsync();
        }

        public Task<bool> DestroyAsync()
        {
            async UniTask<bool> DestroyTask()
            {
                await UniTask.SwitchToMainThread();

                byte? page = null;
                var index = -1;

                for (byte p = 0; p < PlayerInventory.PAGES - 2; p++)
                {
                    var itemJars = Inventory.Inventory.items[p].items;

                    if (itemJars == null)
                    {
                        continue;
                    }

                    index = itemJars.IndexOf(ItemJar);

                    if (index < 0)
                    {
                        continue;
                    }

                    page = p;
                    break;
                }

                if (page == null)
                {
                    return true;
                }

                Inventory.Inventory.removeItem(page.Value, (byte)index);

                if (page.Value < PlayerInventory.SLOTS)
                {
                    Inventory.Player.equipment.sendSlot(page.Value);
                }

                return true;
            }

            return DestroyTask().AsTask();
        }
    }
}