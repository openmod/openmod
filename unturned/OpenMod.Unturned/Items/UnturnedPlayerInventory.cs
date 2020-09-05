using OpenMod.Extensions.Games.Abstractions.Items;
using SDG.Unturned;
using System.Collections;
using System.Collections.Generic;

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
    }
}