using OpenMod.Core.Eventing;
using SDG.Unturned;

namespace OpenMod.Unturned.Events.Vehicles
{
    public abstract class UnturnedVehicleEvent : Event
    {
        protected UnturnedVehicleEvent(InteractableVehicle vehicle)
        {
            Vehicle = vehicle;
        }

        public InteractableVehicle Vehicle { get; }
    }
}
