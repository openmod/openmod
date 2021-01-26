using OpenMod.API.Eventing;

namespace OpenMod.Unturned.Vehicles.Events
{
    /// <summary>
    /// The event that is triggered when a vehicle is going to explode.
    /// </summary>
    public class UnturnedVehicleExplodingEvent : UnturnedVehicleEvent, ICancellableEvent
    {
        public bool IsCancelled { get; set; }

        public UnturnedVehicleExplodingEvent(UnturnedVehicle vehicle) : base(vehicle)
        {

        }
    }
}
