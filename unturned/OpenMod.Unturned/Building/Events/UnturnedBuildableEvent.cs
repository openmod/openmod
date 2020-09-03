using OpenMod.Core.Eventing;
using OpenMod.Extensions.Games.Abstractions.Building;

namespace OpenMod.Unturned.Building.Events
{
    public abstract class UnturnedBuildableEvent : Event, IBuildableEvent
    {
        public UnturnedBuildable Buildable { get; }

        IBuildable IBuildableEvent.Buildable => Buildable;

        protected UnturnedBuildableEvent(UnturnedBuildable buildable)
        {
            Buildable = buildable;
        }
    }
}
