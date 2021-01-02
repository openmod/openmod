using OpenMod.Extensions.Games.Abstractions.Items;
using OpenMod.Rust.Serialization;

namespace OpenMod.Rust.Items
{
    public class RustItemState : IItemState
    {
        private readonly Item m_Item;

        public RustItemState(Item item)
        {
            m_Item = item;
        }

        public double ItemQuality
        {
            get { return m_Item.condition; }
        }

        public double ItemDurability
        {
            get { return m_Item.maxCondition; }
        }

        public double ItemAmount
        {
            get { return m_Item.amount; }
        }

        public byte[] StateData
        {
            get { return BaseNetworkableSerializer.Serialize(m_Item.GetHeldEntity(), out _); }
        }
    }
}
