using OpenMod.Extensions.Games.Abstractions.Items;
using System;
using System.Collections;
using System.Collections.Generic;

namespace OpenMod.Unturned.Items
{
    public class UnturnedPlayerInventoryPage : IInventoryPage
    {
        public SDG.Unturned.Items Page { get; }

        public byte PageIndex { get; }

        public IInventory Inventory { get; set; }

        public UnturnedPlayerInventoryPage(IInventory inventory, byte pageIndex, SDG.Unturned.Items page)
        {
            Inventory = inventory;
            Page = page;
            PageIndex = pageIndex;
        }

        public IEnumerator<IInventoryItem> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => Items.Count;

        public string Name
        {
            get
            {
                switch (PageIndex)
                {
                    case 0:
                        return "Primary slot";
                    case 1:
                        return "Secondary slot";
                    case 2:
                        return "Hands inventory";
                    case 3:
                        return "Backpack";
                    case 4:
                        return "Vest";
                    case 5:
                        return "Shirt";
                    case 6:
                        return "Pants";
                    default:
                        throw new ArgumentOutOfRangeException(nameof(PageIndex), $"Invalid player inventory page id: {PageIndex}");
                }
            }
        }

        public int Capacity => Page.height * Page.width;

        public bool IsReadOnly { get; } = false;

        public bool IsFull => Page.getItemCount() >= Capacity;

        public IReadOnlyCollection<IInventoryItem> Items
        {
            get
            {
                var items = new List<UnturnedInventoryItem>();
                foreach (var itemJar in Page.items)
                {
                    items.Add(new UnturnedInventoryItem(itemJar, new UnturnedItem(itemJar.item)));
                }

                return items;
            }
        }
    }
}