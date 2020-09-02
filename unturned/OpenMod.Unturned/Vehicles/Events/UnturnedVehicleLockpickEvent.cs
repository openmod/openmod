using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;
using SDG.Unturned;

namespace OpenMod.Unturned.Vehicles.Events
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
