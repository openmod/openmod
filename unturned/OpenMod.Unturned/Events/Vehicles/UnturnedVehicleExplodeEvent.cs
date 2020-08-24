using OpenMod.API.Eventing;
using SDG.Unturned;

namespace OpenMod.Unturned.Events.Vehicles
{
    public class UnturnedVehicleExplodeEvent : UnturnedVehicleEvent, ICancellableEvent
    {
        public bool IsCancelled { get; set; }

        public UnturnedVehicleExplodeEvent(InteractableVehicle vehicle) : base(vehicle)
        {

        }
    }
}
