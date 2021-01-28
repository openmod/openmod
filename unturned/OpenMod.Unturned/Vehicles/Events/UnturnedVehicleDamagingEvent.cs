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
        /// <summary>
        /// Gets the ID of the entity causing the damage.
        /// </summary>
        public CSteamID? Instigator { get; }

        /// <summary>
        /// Gets or sets the total damage to be dealt.
        /// </summary>
        public ushort PendingTotalDamage { get; set; }

        /// <summary>
        /// Gets the damage origin.
        /// </summary>
        public EDamageOrigin DamageOrigin { get; }

        /// <summary>
        /// If set to true, the damage will not explode the vehicle if its greater than the current health.
        /// </summary>
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
