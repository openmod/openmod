namespace OpenMod.Extensions.Games.Abstractions.Items
{
    public interface IItemState
    {
        double ItemQuality { get; }

        double ItemDurability { get; }

        double ItemAmount { get; }

        byte[] StateData { get; }
    }
}