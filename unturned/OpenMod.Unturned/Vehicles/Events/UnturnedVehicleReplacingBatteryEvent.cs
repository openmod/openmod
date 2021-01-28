using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;

namespace OpenMod.Unturned.Vehicles.Events
{
    /// <summary>
    /// The event that is triggered when a player is replacing a vehicles battery.
    /// </summary>
    public class UnturnedVehicleReplacingBatteryEvent : UnturnedVehicleEvent, ICancellableEvent
    {
        /// <summary>
        /// Gets the player replacing the battery.
        /// </summary>
        public UnturnedPlayer Instigator { get; }

        /// <summary>
        /// Gets the new battery quality value.
        /// </summary>
        public byte BatteryQuality { get; }

        public bool IsCancelled { get; set; }

        public UnturnedVehicleReplacingBatteryEvent(UnturnedPlayer instigator, UnturnedVehicle vehicle, byte batteryQuality) : base(vehicle)
        {
            Instigator = instigator;
            BatteryQuality = batteryQuality;
        }
    }
}
