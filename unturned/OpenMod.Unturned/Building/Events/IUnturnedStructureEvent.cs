using OpenMod.Extensions.Games.Abstractions.Building;

namespace OpenMod.Unturned.Building.Events
{
    public interface IUnturnedStructureEvent : IBuildableEvent
    {
        new UnturnedStructureBuildable Buildable { get; }
    }
}
