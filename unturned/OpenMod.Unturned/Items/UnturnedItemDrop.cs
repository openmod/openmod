using System.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Items;
using OpenMod.Extensions.Games.Abstractions.Transforms;
using OpenMod.UnityEngine.Transforms;
using SDG.Unturned;

namespace OpenMod.Unturned.Items
{
    public class UnturnedItemDrop : IItemDrop
    {
        public UnturnedItemDrop(ItemRegion region, ItemDrop itemDrop)
        {
            Item = new UnturnedItem(itemDrop.interactableItem.item);
            Transform = new UnityTransform(itemDrop.model);
            Region = region;
        }

        public IItem Item { get; }

        public IWorldTransform Transform { get; }

        public ItemRegion Region { get; }

        public Task<bool> DestroyAsync()
        {
            // todo
            throw new System.NotImplementedException();
        }
    }
}