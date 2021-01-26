using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;

namespace OpenMod.Unturned.Vehicles.Events
{
    /// <summary>
    /// The event that is triggered when a player is siphoning a vehicle.
    /// </summary>
    public class UnturnedVehicleSiphoningEvent : UnturnedVehicleEvent, ICancellableEvent
    {
        /// <value>
        /// The player siphoning the vehicle.
        /// </value>
        public UnturnedPlayer Instigator { get; }

        /// <value>
        /// The amount to siphone.
        /// </value>
        public ushort DesiredAmount { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedVehicleSiphoningEvent(UnturnedVehicle vehicle, UnturnedPlayer instigator, ushort desiredAmount) : base(vehicle)
        {
            Instigator = instigator;
            DesiredAmount = desiredAmount;
        }
    }
}
