using OpenMod.API.Eventing;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Vehicles.Events
{
    /// <summary>
    /// The event that is triggered when damage is being dealt to a vehicle.
    /// </summary>
    public class UnturnedVehicleDamagingEvent : UnturnedVehicleEvent, ICancellableEvent
    {
        /// <value>
        /// The ID of the entity causing the damage.
        /// </value>
        public CSteamID? Instigator { get; }

        /// <value>
        /// The total damage to be dealt.
        /// </value>
        public ushort PendingTotalDamage { get; set; }

        /// <value>
        /// The damage origin.
        /// </value>
        public EDamageOrigin DamageOrigin { get; }

        /// <value>
        /// True, the damage will not explode the vehicle if its greater than the current health.
        /// </value>
        public bool CanRepair { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedVehicleDamagingEvent(UnturnedVehicle vehicle, CSteamID? instigator, ushort pendingTotalDamage, EDamageOrigin damageOrigin, bool canRepair) : base(vehicle)
        {
            Instigator = instigator;
            PendingTotalDamage = pendingTotalDamage;
            DamageOrigin = damageOrigin;
            CanRepair = canRepair;
        }
    }
}
