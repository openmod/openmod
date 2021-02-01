using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OpenMod.Extensions.Games.Abstractions.Items;

namespace OpenMod.Rust.Items
{
    public class RustInventoryPage : IInventoryPage
    {
        public ItemContainer ItemContainer { get; }

        public RustInventoryPage(ItemContainer itemContainer, IInventory inventory, string? name)
        {
            ItemContainer = itemContainer;
            Inventory = inventory;
            Name = name ?? "Unknown name";
        }

        public IEnumerator<IInventoryItem> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count
        {
            get { return Items.Count; }
        }

        public IInventory Inventory { get; }

        public string Name { get; }

        public int Capacity
        {
            get { return ItemContainer.capacity; }
        }

        public bool IsReadOnly
        {
            get { return ItemContainer.IsLocked(); }
        }

        public bool IsFull
        {
            get { return ItemContainer.IsFull(); }
        }

        public IReadOnlyCollection<IInventoryItem> Items
        {
            get
            {
                return ItemContainer.itemList
                    .Select(x => new RustInventoryItem(new RustItem(x)))
                    .ToList();
            }
        }
    }
}
