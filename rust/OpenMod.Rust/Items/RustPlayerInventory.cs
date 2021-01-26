using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enum = System.Enum;
using OpenMod.Extensions.Games.Abstractions.Items;

namespace OpenMod.Rust.Items
{
    public class RustPlayerInventory : IInventory
    {
        public PlayerInventory PlayerInventory { get; }

        public RustPlayerInventory(PlayerInventory playerInventory)
        {
            PlayerInventory = playerInventory;
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

        public IReadOnlyCollection<IInventoryPage> Pages
        {
            get
            {
                var types = Enum.GetValues(typeof(PlayerInventory.Type));
                var list = new List<RustInventoryPage>(capacity: types.Length);

                foreach (PlayerInventory.Type type in types)
                {
                    ItemContainer container = PlayerInventory.GetContainer(type);

                    if (container != null)
                    {
                        var name = Enum.GetName(typeof(PlayerInventory.Type), type);
                        var page = new RustInventoryPage(container, this, name);
                        list.Add(page);
                    }
                }

                return list;
            }
        }

        internal bool GiveItem(Item item, ItemContainer? container = null)
        {
            return PlayerInventory.GiveItem(item, container);
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
