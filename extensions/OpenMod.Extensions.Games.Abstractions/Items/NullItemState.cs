namespace OpenMod.Extensions.Games.Abstractions.Items
{
    public class NullItemState : IItemState
    {
        private static NullItemState m_ItemState;
        public static NullItemState Instance
        {
            get
            {
                return m_ItemState ??= new NullItemState();
            }
        }

        private NullItemState()
        {

        }

        public double ItemQuality { get; } = 0;

        public double ItemDurability { get; } = 0;

        public double ItemAmount { get; } = 0;

        public byte[] StateData { get; } = null;
    }
}