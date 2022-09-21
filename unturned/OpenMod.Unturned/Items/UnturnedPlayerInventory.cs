using System;
using OpenMod.Extensions.Games.Abstractions.Items;
using SDG.Unturned;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OpenMod.Unturned.Items
{
    public class UnturnedPlayerInventory : IInventory
    {
        public PlayerInventory Inventory { get; }

        public Player Player { get; }

        public UnturnedPlayerInventory(Player player)
        {
            Inventory = player.inventory;
            Player = player;
        }

        public IEnumerator<IInventoryPage> GetEnumerator()
        {
            return Pages.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => Pages.Count;

        public IReadOnlyCollection<IInventoryPage> Pages
        {
            get
            {
                var list = new List<UnturnedPlayerInventoryPage>();
                for (byte page = 0; page < PlayerInventory.PAGES; page++)
                {
                    if (page == PlayerInventory.AREA || page == PlayerInventory.STORAGE)
                    {
                        continue;
                    }

                    list.Add(new UnturnedPlayerInventoryPage(this, page, Inventory.items[page]));
                }

                return list;
            }
        }

        public void Clear()
        {
            for (byte page = 0; page < PlayerInventory.PAGES - 2; page++)
            {
                for (int index = Inventory.getItemCount(page) - 1; index >= 0; index--)
                {
                    Inventory.removeItem(page, (byte)index);
                }
            }
            Player.clothing.updateClothes(0, 0, Array.Empty<byte>(), 0, 0, Array.Empty<byte>(), 0, 0, Array.Empty<byte>(), 0, 0, Array.Empty<byte>(), 0, 0, Array.Empty<byte>(), 0, 0, Array.Empty<byte>(), 0, 0, Array.Empty<byte>());
            Player.equipment.sendSlot(0);
            Player.equipment.sendSlot(1);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return (obj as UnturnedPlayerInventory)?.Player == Player;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Player.GetHashCode();
        }

        public bool IsFull
        {
            get
            {
                return Pages.All(d => d.IsFull);
            }
        }
    }
}