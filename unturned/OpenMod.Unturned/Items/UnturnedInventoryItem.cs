using System;
using System.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Items;
using SDG.Unturned;

namespace OpenMod.Unturned.Items
{
    public class UnturnedInventoryItem : IInventoryItem
    {
        public UnturnedInventoryItem(ItemJar itemJar, UnturnedItem item)
        {
            ItemJar = itemJar;
            Item = item;
        }

        public ItemJar ItemJar { get; }

        public IItem Item { get; }

        public Task DropAsync()
        {
            throw new NotImplementedException();
        }

        public Task DestroyAsync()
        {
            throw new NotImplementedException();
        }
    }
}