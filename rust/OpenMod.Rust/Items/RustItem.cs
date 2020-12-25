using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Items;

namespace OpenMod.Rust.Items
{
    // todo: implement OpenMod.Extensions.Games.Abstractions.Items.IHasInventory
    public class RustItem : IItem
    {
        public Item Item { get; }

        public RustItem(Item item)
        {
            Item = item;
            Asset = new RustItemAsset(Item.info);
            State = new RustItemState(Item);
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
    }
}
