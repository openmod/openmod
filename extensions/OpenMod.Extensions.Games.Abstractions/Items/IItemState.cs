using JetBrains.Annotations;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    public interface IItemState
    {
        double ItemQuality { get; }

        double ItemAmount { get; }

        [CanBeNull]
        byte[] StateData { get; }
    }
}