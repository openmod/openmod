using OpenMod.API.Eventing;
using OpenMod.Unturned.Entities;
using SDG.Unturned;
using UnityEngine;

namespace OpenMod.Unturned.Events.Vehicles
{
    public class UnturnedVehicleCarjackEvent : UnturnedVehicleEvent, ICancellableEvent
    {
        public UnturnedPlayer Instigator { get; }

        public Vector3 Force { get; set; }

        public Vector3 Torque { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedVehicleCarjackEvent(InteractableVehicle vehicle, UnturnedPlayer instigator, Vector3 force, Vector3 torque) : base(vehicle)
        {
            Instigator = instigator;
            Force = force;
            Torque = torque;
        }
    }
}
