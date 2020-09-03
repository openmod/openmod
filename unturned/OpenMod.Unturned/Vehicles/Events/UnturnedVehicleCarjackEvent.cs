using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;
using System.Numerics;

namespace OpenMod.Unturned.Vehicles.Events
{
    public class UnturnedVehicleCarjackEvent : UnturnedVehicleEvent, ICancellableEvent
    {
        public UnturnedPlayer Instigator { get; }

        public Vector3 Force { get; set; }

        public Vector3 Torque { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedVehicleCarjackEvent(UnturnedVehicle vehicle, UnturnedPlayer instigator, Vector3 force, Vector3 torque) : base(vehicle)
        {
            Instigator = instigator;
            Force = force;
            Torque = torque;
        }
    }
}
