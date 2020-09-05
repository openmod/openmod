using OpenMod.API.Eventing;

namespace OpenMod.Unturned.Vehicles.Events
{
    public class UnturnedVehicleExplodingEvent : UnturnedVehicleEvent, ICancellableEvent
    {
        public bool IsCancelled { get; set; }

        public UnturnedVehicleExplodingEvent(UnturnedVehicle vehicle) : base(vehicle)
        {

        }
    }
}
