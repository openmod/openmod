using OpenMod.API.Eventing;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Events.Vehicles
{
    public class UnturnedVehicleDamageEvent : UnturnedVehicleEvent, ICancellableEvent
    {
        public CSteamID Instigator { get; }

        public ushort PendingTotalDamage { get; set; }

        public EDamageOrigin DamageOrigin { get; }

        public bool CanRepair { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedVehicleDamageEvent(InteractableVehicle vehicle, CSteamID instigator, ushort pendingTotalDamage, EDamageOrigin damageOrigin, bool canRepair) : base(vehicle)
        {
            Instigator = instigator;
            PendingTotalDamage = pendingTotalDamage;
            DamageOrigin = damageOrigin;
            CanRepair = canRepair;
        }
    }
}
