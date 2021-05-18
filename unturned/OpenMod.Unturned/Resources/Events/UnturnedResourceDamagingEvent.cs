using OpenMod.API.Eventing;
using OpenMod.Core.Eventing;
using OpenMod.Unturned.Players;

using SDG.Unturned;

using Steamworks;

namespace OpenMod.Unturned.Resources.Events
{
    public class UnturnedResourceDamagingEvent : Event, ICancellableEvent
    {
        public ResourceSpawnpoint ResourceSpawnpoint { get; }

        public ushort DamageAmount { get; set; }

        public EDamageOrigin DamageOrigin { get; }

        public UnturnedPlayer? Instigator { get; }

        public CSteamID InstigatorId { get; }

        public bool IsCancelled { get; set; }

        public UnturnedResourceDamagingEvent(ResourceSpawnpoint resourceSpawnpoint, ushort damageAmount, EDamageOrigin damageOrigin,
            UnturnedPlayer? instigator, CSteamID instigatorId)
        {
            ResourceSpawnpoint = resourceSpawnpoint;
            DamageAmount = damageAmount;
            DamageOrigin = damageOrigin;
            Instigator = instigator;
            InstigatorId = instigatorId;
        }
    }
}