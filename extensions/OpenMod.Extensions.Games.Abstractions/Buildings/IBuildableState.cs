using JetBrains.Annotations;

namespace OpenMod.Extensions.Games.Abstractions.Buildings
{
    public interface IBuildableState
    {
        ushort Health { get; }

        [CanBeNull]
        byte[] State { get; }
    }
}