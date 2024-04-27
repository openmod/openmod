using OpenMod.Extensions.Games.Abstractions.Items;
using SDG.Unturned;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Items
{
    public class UnturnedItem : IItem
    {
        public delegate Task<bool> DestroyItem();

        private readonly DestroyItem m_DestroyItem;

        // ReSharper disable once MemberCanBePrivate.Global
        public Item Item { get; }

        public UnturnedItem(Item item, DestroyItem destroyItem)
        {
            m_DestroyItem = destroyItem;
            Item = item;
            Asset = new UnturnedItemAsset((ItemAsset)Assets.find(EAssetType.ITEM, item.id));
            State = new UnturnedItemState(item);
        }

        public string ItemInstanceId
        {
            get => Item.GetHashCode().ToString();
        }

        public IItemAsset Asset { get; }

        public IItemState State { get; }

        public Task SetItemQualityAsync(double quality)
        {
            //todo: needs replication if equipped by a player
            Item.quality = (byte)quality;
            return Task.CompletedTask;
        }

        public Task SetItemAmountAsync(double amount)
        {
            //todo: needs replication if equipped by a player
            Item.amount = (byte)amount;
            return Task.CompletedTask;
        }

        public Task SetItemDurabilityAsync(double durability)
        {
            return SetItemQualityAsync(durability);
        }

        public Task<bool> DestroyAsync()
        {
            return m_DestroyItem();
        }
    }
}