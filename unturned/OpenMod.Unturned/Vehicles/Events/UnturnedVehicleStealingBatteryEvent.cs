using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;

namespace OpenMod.Unturned.Vehicles.Events
{
    /// <summary>
    /// The event that is triggered when a player is removing a battery.
    /// </summary>
    public class UnturnedVehicleStealingBatteryEvent : UnturnedVehicleEvent, ICancellableEvent
    {
        /// <summary>
        /// Gets the player removing the battery.
        /// </summary>
        public UnturnedPlayer Instigator { get; }

        public bool IsCancelled { get; set; }

        public UnturnedVehicleStealingBatteryEvent(UnturnedPlayer instigator, UnturnedVehicle vehicle) : base(vehicle)
        {
            Instigator = instigator;
        }
    }
}
