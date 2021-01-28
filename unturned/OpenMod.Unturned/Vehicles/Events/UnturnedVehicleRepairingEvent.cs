using OpenMod.API.Eventing;
using Steamworks;

namespace OpenMod.Unturned.Vehicles.Events
{
    /// <summary>
    /// The event that is triggered when a player is repairing a vehicle.
    /// </summary>
    public class UnturnedVehicleRepairingEvent : UnturnedVehicleEvent, ICancellableEvent
    {
        /// <summary>
        /// Gets the player repairing the vehicle.
        /// </summary>
        public CSteamID Instigator { get; }

        /// <summary>
        /// Gets or sets the amount of health to restore.
        /// </summary>
        public ushort PendingTotalHealing { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedVehicleRepairingEvent(UnturnedVehicle vehicle, CSteamID instigator, ushort pendingTotalHealing) : base(vehicle)
        {
            Instigator = instigator;
            PendingTotalHealing = pendingTotalHealing;
        }
    }
}
