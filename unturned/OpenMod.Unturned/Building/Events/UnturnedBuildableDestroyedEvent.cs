using OpenMod.Extensions.Games.Abstractions.Building;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedBuildableDestroyedEvent : UnturnedBuildableEvent, IBuildableDestroyedEvent
    {
        public UnturnedBuildableDestroyedEvent(UnturnedBuildable buildable) : base(buildable)
        {
        }
    }
}
