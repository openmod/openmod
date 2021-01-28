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
        /// <summary>
        /// Gets the Steam ID of the entity causing the damage.
        /// </summary>
        public CSteamID? Instigator { get; }

        /// <summary>
        /// Gets the index of the tire that is getting damaged.
        /// </summary>
        public int TireIndex { get; }

        /// <summary>
        /// Gets the damage origin.
        /// </summary>
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
