using OpenMod.Core.Eventing;
using OpenMod.Extensions.Games.Abstractions.Vehicles;

namespace OpenMod.Unturned.Vehicles.Events
{
    public abstract class UnturnedVehicleEvent : Event, IVehicleEvent
    {
        public UnturnedVehicle Vehicle { get; }

        IVehicle IVehicleEvent.Vehicle => Vehicle;

        protected UnturnedVehicleEvent(UnturnedVehicle vehicle)
        {
            Vehicle = vehicle;
        }
    }
}
