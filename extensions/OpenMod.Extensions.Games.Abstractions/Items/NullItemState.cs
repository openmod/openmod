namespace OpenMod.Extensions.Games.Abstractions.Items
{
    /// <summary>
    /// Represents the null item state for items that do not have a state.
    /// </summary>
    public class NullItemState : IItemState
    {
        private static NullItemState? s_ItemState;
        public static NullItemState Instance
        {
            get
            {
                return s_ItemState ??= new NullItemState();
            }
        }

        private NullItemState()
        {

        }

        public double ItemQuality { get; } = 0;

        public double ItemDurability { get; } = 0;

        public double ItemAmount { get; } = 0;

        public byte[]? StateData { get; } = null;
    }
}