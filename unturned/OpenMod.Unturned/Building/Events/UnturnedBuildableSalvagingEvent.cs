using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;
using Steamworks;

namespace OpenMod.Unturned.Building.Events
{
    public abstract class UnturnedBuildableSalvagingEvent : UnturnedBuildableEvent, ICancellableEvent
    {
        public UnturnedPlayer Instigator { get; }

        public CSteamID InstigatorSteamId { get; }

        public bool IsCancelled { get; set; }

        protected UnturnedBuildableSalvagingEvent(UnturnedBuildable buildable, UnturnedPlayer instigator, CSteamID instigatorSteamId) : base(buildable)
        {
            Instigator = instigator;
            InstigatorSteamId = instigatorSteamId;
        }
    }
}
