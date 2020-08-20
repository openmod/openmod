using JetBrains.Annotations;

namespace OpenMod.Extensions.Games.Abstractions.Building
{
    public interface IBuildableState
    {
        double Health { get; }

        [CanBeNull]
        byte[] State { get; }
    }
}