using OpenMod.API.Eventing;

namespace OpenMod.Unturned.Vehicles.Events
{
    public class UnturnedVehicleExplodeEvent : UnturnedVehicleEvent, ICancellableEvent
    {
        public bool IsCancelled { get; set; }

        public UnturnedVehicleExplodeEvent(UnturnedVehicle vehicle) : base(vehicle)
        {

        }
    }
}
