using Cysharp.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Items;
using System.Threading.Tasks;

namespace OpenMod.Rust.Items
{
    public class RustItem : IItem, IHasInventory
    {
        public Item Item { get; }

        public RustItem(Item item)
        {
            Item = item;
            Asset = new RustItemAsset(Item.info);
            State = new RustItemState(Item);

            if (Item.contents != null)
            {
                Inventory = new RustItemContainerInventory(Item.contents);
            }
        }

        public string ItemInstanceId
        {
            get { return Item.uid.ToString(); }
        }

        public IItemAsset Asset { get; }

        public IItemState State { get; }

        public Task SetItemQualityAsync(double quality)
        {
            async UniTask SetItemQualityTask()
            {
                await UniTask.SwitchToMainThread();

                Item.condition = (float) quality;
            }

            return SetItemQualityTask().AsTask();
        }

        public Task SetItemAmountAsync(double amount)
        {
            async UniTask SetItemAmountTask()
            {
                await UniTask.SwitchToMainThread();

                Item.amount = (int) amount;
                Item.MarkDirty();
            }

            return SetItemAmountTask().AsTask();
        }

        public Task SetItemDurabilityAsync(double durability)
        {
            async UniTask SetItemDurabilityTask()
            {
                await UniTask.SwitchToMainThread();

                Item.maxCondition = (int) durability;
            }

            return SetItemDurabilityTask().AsTask();
        }

        public IInventory? Inventory { get; }
    }
}
