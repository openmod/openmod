using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedPlantHarvestingEvent : UnturnedBuildableEvent, IUnturnedBarricadeEvent, ICancellableEvent
    {
        public bool IsCancelled { get; set; }

        public UnturnedPlayer? Instigator { get; }

        public new UnturnedBarricadeBuildable Buildable => (UnturnedBarricadeBuildable)base.Buildable;

        public UnturnedPlantHarvestingEvent(UnturnedBuildable buildable, UnturnedPlayer? instigator) : base(buildable)
        {
            Instigator = instigator;
        }
    }
}
