using OpenMod.Extensions.Games.Abstractions.Building;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedBuildableDeployedEvent : UnturnedBuildableEvent, IBuildableDeployedEvent
    {
        public UnturnedBuildableDeployedEvent(UnturnedBuildable buildable) : base(buildable)
        {
        }
    }
}
