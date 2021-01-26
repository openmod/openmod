using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;
using System.Numerics;

namespace OpenMod.Unturned.Vehicles.Events
{
    /// <summary>
    /// The event that is triggered when a player is trying to carjack a vehicle.
    /// </summary>
    public class UnturnedVehicleCarjackingEvent : UnturnedVehicleEvent, ICancellableEvent
    {
        /// <value>
        /// The player trying to carjack.
        /// </value>
        public UnturnedPlayer Instigator { get; }

        /// <value>
        /// The force to be applied.
        /// </value>
        public Vector3 Force { get; set; }

        /// <value>
        /// The torque to be applied.
        /// </value>
        public Vector3 Torque { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedVehicleCarjackingEvent(UnturnedVehicle vehicle, UnturnedPlayer instigator, Vector3 force, Vector3 torque) : base(vehicle)
        {
            Instigator = instigator;
            Force = force;
            Torque = torque;
        }
    }
}
