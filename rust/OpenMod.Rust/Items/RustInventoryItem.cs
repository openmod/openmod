using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Items;

namespace OpenMod.Rust.Items
{
    public class RustInventoryItem : IInventoryItem
    {
        public RustItem Item { get; }

        public RustInventoryItem(RustItem item)
        {
            Item = item;
        }

        IItem IItemObject.Item
        {
            get { return Item; }
        }

        public Task DropAsync()
        {
            async UniTask DropTask()
            {
                await UniTask.SwitchToMainThread();

                BasePlayer owner = Item.Item.GetOwnerPlayer();
                Item.Item.Drop(owner.GetDropPosition(), owner.GetDropVelocity());
            }

            return DropTask().AsTask();
        }

        public Task DestroyAsync()
        {
            async UniTask DestroyTask()
            {
                await UniTask.SwitchToMainThread();

                Item.Item.GetOwnerPlayer().inventory.Take(null, Item.Item.info.itemid, Item.Item.amount);
            }

            return DestroyTask().AsTask();
        }
    }
}
