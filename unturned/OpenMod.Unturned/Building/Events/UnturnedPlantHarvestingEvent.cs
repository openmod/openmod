using System;
using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;
using Steamworks;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedPlantHarvestingEvent : UnturnedBuildableEvent, IUnturnedBarricadeEvent, ICancellableEvent
    {
        public bool IsCancelled { get; set; }

        [Obsolete("Use Instigator.SteamId")]
        public CSteamID InstigatorSteamId { get { return Instigator?.SteamId ?? CSteamID.Nil; } }

        public UnturnedPlayer? Instigator { get; }

        public new UnturnedBarricadeBuildable Buildable => (UnturnedBarricadeBuildable)base.Buildable;

        public UnturnedPlantHarvestingEvent(UnturnedBuildable buildable, UnturnedPlayer? instigator) : base(buildable)
        {
            Instigator = instigator;
        }
    }
}
