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
        /// <summary>
        /// Gets the player trying to carjack.
        /// </summary>
        public UnturnedPlayer Instigator { get; }

        /// <summary>
        /// Gets or sets the force to be applied.
        /// </summary>
        public Vector3 Force { get; set; }

        /// <summary>
        /// Gets or sets the torque to be applied.
        /// </summary>
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
