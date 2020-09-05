using OpenMod.Extensions.Games.Abstractions.Items;
using SDG.Unturned;

namespace OpenMod.Unturned.Items
{
    public class UnturnedItemState : IItemState
    {
        public Item Item { get; }

        public UnturnedItemState(Item item)
        {
            Item = item;
        }

        public double ItemQuality => Item.quality;

        public double ItemDurability => Item.durability;

        public double ItemAmount => Item.amount;

        /* Item.state and Item.metadata are the same thing */
        public byte[] StateData => Item.state;
    }
}