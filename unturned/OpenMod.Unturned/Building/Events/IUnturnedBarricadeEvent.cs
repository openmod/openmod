using OpenMod.Extensions.Games.Abstractions.Building;

namespace OpenMod.Unturned.Building.Events
{
    public interface IUnturnedBarricadeEvent : IBuildableEvent
    {
        new UnturnedBarricadeBuildable Buildable { get; }
    }
}
