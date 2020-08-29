using OpenMod.API.Eventing;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Extensions.Games.Abstractions.Vehicles;
using OpenMod.Unturned.Entities;
using OpenMod.Unturned.Vehicles;
using SDG.Unturned;
using UnityEngine;

namespace OpenMod.Unturned.Events.Vehicles
{
    public class UnturnedPlayerExitVehicleEvent : UnturnedPlayerEvent, IPlayerExitVehicleEvent
    {
        public InteractableVehicle Vehicle { get; }

        IVehicle IVehicleEvent.Vehicle => new UnturnedVehicle(Vehicle);

        public Vector3 PendingLocation { get; set; }

        public float PendingYaw { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedPlayerExitVehicleEvent(UnturnedPlayer player, InteractableVehicle vehicle, Vector3 pendingLocation, float pendingYaw) : base(player)
        {
            Vehicle = vehicle;
            PendingLocation = pendingLocation;
            PendingYaw = pendingYaw;
        }
    }
}