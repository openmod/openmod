using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;

namespace OpenMod.Unturned.Vehicles.Events
{
    /// <summary>
    /// The event that is triggered when a player is replacing a vehicles battery.
    /// </summary>
    public class UnturnedVehicleReplacingBatteryEvent : UnturnedVehicleEvent, ICancellableEvent
    {
        /// <value>
        /// The player replacing the battery.
        /// </value>
        public UnturnedPlayer Instigator { get; }

        /// <value>
        /// The new battery quality.
        /// </value>
        public byte BatteryQuality { get; }

        public bool IsCancelled { get; set; }

        public UnturnedVehicleReplacingBatteryEvent(UnturnedPlayer instigator, UnturnedVehicle vehicle, byte batteryQuality) : base(vehicle)
        {
            Instigator = instigator;
            BatteryQuality = batteryQuality;
        }
    }
}
