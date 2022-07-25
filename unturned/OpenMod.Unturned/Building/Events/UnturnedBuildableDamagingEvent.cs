using System;
using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Building.Events
{
    public abstract class UnturnedBuildableDamagingEvent : UnturnedBuildableEvent, ICancellableEvent
    {
        public ushort DamageAmount { get; set; }

        public EDamageOrigin DamageOrigin { get; }

        public UnturnedPlayer? Instigator { get; }

        [Obsolete("Use Instigator.SteamId")]
        public CSteamID InstigatorId { get { return Instigator?.SteamId ?? CSteamID.Nil; } }

        public bool IsCancelled { get; set; }

        protected UnturnedBuildableDamagingEvent(UnturnedBuildable buildable, ushort damageAmount, EDamageOrigin damageOrigin,
            UnturnedPlayer? instigator) : base(buildable)
        {
            DamageAmount = damageAmount;
            DamageOrigin = damageOrigin;
            Instigator = instigator;
        }
    }
}
