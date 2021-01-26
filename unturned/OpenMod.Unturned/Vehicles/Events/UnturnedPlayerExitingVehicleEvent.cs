using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Extensions.Games.Abstractions.Vehicles;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Players;
using System.Numerics;

namespace OpenMod.Unturned.Vehicles.Events
{
    /// <summary>
    /// The event that is triggered when a player is exitting a vehicle.
    /// </summary>
    public class UnturnedPlayerExitingVehicleEvent : UnturnedPlayerEvent, IPlayerExitingVehicleEvent
    {
        public UnturnedVehicle Vehicle { get; }

        IVehicle IVehicleEvent.Vehicle => Vehicle;

        /// <value>
        /// The exit location to be teleported to.
        /// </value>
        public Vector3 PendingLocation { get; set; }

        /// <value>
        /// The exit yaw to be set.
        /// </value>
        public float PendingYaw { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedPlayerExitingVehicleEvent(UnturnedPlayer player, UnturnedVehicle vehicle, Vector3 pendingLocation, float pendingYaw) : base(player)
        {
            Vehicle = vehicle;
            PendingLocation = pendingLocation;
            PendingYaw = pendingYaw;
        }
    }
}