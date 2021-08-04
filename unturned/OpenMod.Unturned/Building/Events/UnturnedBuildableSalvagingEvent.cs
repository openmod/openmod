using System;
using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;
using Steamworks;

namespace OpenMod.Unturned.Building.Events
{
    public abstract class UnturnedBuildableSalvagingEvent : UnturnedBuildableEvent, ICancellableEvent
    {
        public UnturnedPlayer? Instigator { get; }

        [Obsolete("Use Instigator.SteamId")]
        public CSteamID InstigatorSteamId { get { return Instigator?.SteamId ?? CSteamID.Nil; } }

        public bool IsCancelled { get; set; }

        protected UnturnedBuildableSalvagingEvent(UnturnedBuildable buildable, UnturnedPlayer? instigator)
            : base(buildable)
        {
            Instigator = instigator;
        }
    }
}
