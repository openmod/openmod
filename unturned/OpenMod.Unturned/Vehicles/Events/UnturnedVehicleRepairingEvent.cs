using OpenMod.API.Eventing;
using Steamworks;

namespace OpenMod.Unturned.Vehicles.Events
{
    /// <summary>
    /// The event that is triggered when a player is repairing a vehicle.
    /// </summary>
    public class UnturnedVehicleRepairingEvent : UnturnedVehicleEvent, ICancellableEvent
    {
        /// <value>
        /// The player repairing the vehicle.
        /// </value>
        public CSteamID Instigator { get; }

        /// <value>
        /// The amount of health to restore.
        /// </value>
        public ushort PendingTotalHealing { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedVehicleRepairingEvent(UnturnedVehicle vehicle, CSteamID instigator, ushort pendingTotalHealing) : base(vehicle)
        {
            Instigator = instigator;
            PendingTotalHealing = pendingTotalHealing;
        }
    }
}
