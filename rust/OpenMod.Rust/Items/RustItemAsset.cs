using OpenMod.Extensions.Games.Abstractions.Items;
using System.Globalization;

namespace OpenMod.Rust.Items
{
    public class RustItemAsset : IItemAsset
    {
        public ItemDefinition ItemDefinition { get; }

        public RustItemAsset(ItemDefinition itemDefinition)
        {
            ItemDefinition = itemDefinition;
        }

        public string ItemAssetId
        {
            get { return ItemDefinition.itemid.ToString(); }
        }

        public string ItemName
        {
            get { return ItemDefinition.displayName.translated; }
        }

        public string ItemType
        {
            get { return ItemDefinition.category.ToString().ToLower(CultureInfo.InvariantCulture); }
        }

        public double? MaxQuality
        {
            get { return ItemDefinition.condition.enabled ? ItemDefinition.condition.max : null; }
        }

        public double? MaxAmount
        {
            get { return ItemDefinition.stackable; }
        }

        public double? MaxDurability
        {
            get { return MaxQuality; }
        }
    }
}
