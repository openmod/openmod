using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;

namespace OpenMod.Unturned.Vehicles.Events
{
    /// <summary>
    /// The event that is triggered when a player tries to lockpick a vehicle.
    /// </summary>
    public class UnturnedVehicleLockpickingEvent : UnturnedVehicleEvent, ICancellableEvent
    {
        /// <value>
        /// The player trying to lockpick the vehicle.
        /// </value>
        public UnturnedPlayer Instigator { get; }

        public bool IsCancelled { get; set; }

        public UnturnedVehicleLockpickingEvent(UnturnedVehicle vehicle, UnturnedPlayer instigator) : base(vehicle)
        {
            Instigator = instigator;
        }
    }
}
