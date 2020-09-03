using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Building.Events
{
    public abstract class UnturnedBuildableDamageEvent : UnturnedBuildableEvent, ICancellableEvent
    {
        public ushort DamageAmount { get; set; }

        public EDamageOrigin DamageOrigin { get; }

        public UnturnedPlayer Instigator { get; }

        public CSteamID InstigatorId { get; }

        public bool IsCancelled { get; set; }

        protected UnturnedBuildableDamageEvent(UnturnedBuildable buildable, ushort damageAmount, EDamageOrigin damageOrigin, UnturnedPlayer instigator, CSteamID instigatorId) : base(buildable)
        {
            DamageAmount = damageAmount;
            DamageOrigin = damageOrigin;
            Instigator = instigator;
            InstigatorId = instigatorId;
        }
    }
}
