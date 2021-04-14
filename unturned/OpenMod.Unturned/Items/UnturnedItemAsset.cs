using OpenMod.Extensions.Games.Abstractions.Items;
using SDG.Unturned;
using System.Globalization;

namespace OpenMod.Unturned.Items
{
    public class UnturnedItemAsset : IItemAsset
    {
        public ItemAsset ItemAsset { get; }

        public UnturnedItemAsset(ItemAsset itemAsset)
        {
            ItemAsset = itemAsset;
        }

        public string ItemAssetId => ItemAsset.id.ToString();

        public string ItemName => ItemAsset.itemName ?? ItemAssetId;

        public string ItemType => ItemAsset.type.ToString().ToLower(CultureInfo.InvariantCulture);

        public double? MaxQuality => ItemAsset.qualityMax;

        public double? MaxAmount => ItemAsset.amount;

        public double? MaxDurability => MaxQuality;
    }
}