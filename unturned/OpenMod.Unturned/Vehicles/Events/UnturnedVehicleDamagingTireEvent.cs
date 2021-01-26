using OpenMod.API.Eventing;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Vehicles.Events
{
    /// <summary>
    /// The event that is triggered when damage is dealt to a tire of a vehicle.
    /// </summary>
    public class UnturnedVehicleDamagingTireEvent : UnturnedVehicleEvent, ICancellableEvent
    {
        /// <value>
        /// The ID of the entity causing the damage.
        /// </value>
        public CSteamID? Instigator { get; }

        /// <value>
        /// The index of the tire that is getting damaged.
        /// </value>
        public int TireIndex { get; }

        /// <value>
        /// The damage origin.
        /// </value>
        public EDamageOrigin DamageOrigin { get; }

        public bool IsCancelled { get; set; }

        public UnturnedVehicleDamagingTireEvent(UnturnedVehicle vehicle, CSteamID instigator, int tireIndex, EDamageOrigin damageOrigin) : base(vehicle)
        {
            Instigator = instigator;
            TireIndex = tireIndex;
            DamageOrigin = damageOrigin;
        }
    }
}
