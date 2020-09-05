using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;

namespace OpenMod.Unturned.Vehicles.Events
{
    public class UnturnedVehicleLockpickingEvent : UnturnedVehicleEvent, ICancellableEvent
    {
        public UnturnedPlayer Instigator { get; }

        public bool IsCancelled { get; set; }

        public UnturnedVehicleLockpickingEvent(UnturnedVehicle vehicle, UnturnedPlayer instigator) : base(vehicle)
        {
            Instigator = instigator;
        }
    }
}
