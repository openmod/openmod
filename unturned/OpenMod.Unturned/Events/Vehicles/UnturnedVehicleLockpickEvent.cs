using OpenMod.API.Eventing;
using OpenMod.Unturned.Entities;
using SDG.Unturned;

namespace OpenMod.Unturned.Events.Vehicles
{
    public class UnturnedVehicleLockpickEvent : UnturnedVehicleEvent, ICancellableEvent
    {
        public UnturnedPlayer Instigator { get; }

        public bool IsCancelled { get; set; }

        public UnturnedVehicleLockpickEvent(InteractableVehicle vehicle, UnturnedPlayer instigator) : base(vehicle)
        {
            Instigator = instigator;
        }
    }
}
