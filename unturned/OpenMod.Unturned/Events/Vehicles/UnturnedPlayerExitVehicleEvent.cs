using OpenMod.API.Eventing;
using OpenMod.Unturned.Entities;
using SDG.Unturned;
using UnityEngine;

namespace OpenMod.Unturned.Events.Vehicles
{
    public class UnturnedPlayerExitVehicleEvent : UnturnedPlayerEvent, ICancellableEvent
    {
        public InteractableVehicle Vehicle { get; }

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