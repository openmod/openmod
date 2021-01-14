using System.Globalization;
using OpenMod.Extensions.Games.Abstractions.Items;

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
    }
}
