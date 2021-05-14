using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;

using Steamworks;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedPlantHarvestingEvent : UnturnedBuildableEvent, IUnturnedBarricadeEvent, ICancellableEvent
    {
        public bool IsCancelled { get; set; }

        public CSteamID InstigatorSteamId { get; }

        public UnturnedPlayer? Instigator { get; }

        public new UnturnedBarricadeBuildable Buildable => (UnturnedBarricadeBuildable) base.Buildable;

        public UnturnedPlantHarvestingEvent(UnturnedBuildable buildable, UnturnedPlayer instigator, CSteamID instigatorSteamId) : base(buildable)
        {
            Instigator = instigator;
            InstigatorSteamId = instigatorSteamId;
        }
    }
}
