using System.Collections;
using System.Collections.Generic;
using OpenMod.Extensions.Games.Abstractions.Items;

namespace OpenMod.Rust.Items
{
    public class RustItemContainerInventory : IInventory
    {
        public ItemContainer ItemContainer { get; }

        public RustItemContainerInventory(ItemContainer itemContainer)
        {
            ItemContainer = itemContainer;
            Pages = new IInventoryPage[]
            {
                new RustInventoryPage(ItemContainer, this, ItemContainer?.parent.info.displayName.translated)
            };
        }

        public IEnumerator<IInventoryPage> GetEnumerator()
        {
            return Pages.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count
        {
            get { return Pages.Count; }
        }

        public IReadOnlyCollection<IInventoryPage> Pages { get; }
    }
}
