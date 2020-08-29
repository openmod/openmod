using OpenMod.Core.Eventing;
using OpenMod.Extensions.Games.Abstractions.Vehicles;
using OpenMod.Unturned.Vehicles;
using SDG.Unturned;

namespace OpenMod.Unturned.Events.Vehicles
{
    public abstract class UnturnedVehicleEvent : Event, IVehicleEvent
    {
        protected UnturnedVehicleEvent(InteractableVehicle vehicle)
        {
            Vehicle = vehicle;
        }

        public InteractableVehicle Vehicle { get; }

        IVehicle IVehicleEvent.Vehicle => new UnturnedVehicle(Vehicle);
    }
}
