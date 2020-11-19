using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;

namespace OpenMod.Unturned.Vehicles.Events
{
    public class UnturnedVehicleStealingBatteryEvent : UnturnedVehicleEvent, ICancellableEvent
    {
        public UnturnedPlayer Instigator { get; }

        public bool IsCancelled { get; set; }

        public UnturnedVehicleStealingBatteryEvent(UnturnedPlayer instigator, UnturnedVehicle vehicle) : base(vehicle)
        {
            Instigator = instigator;
        }
    }
}
