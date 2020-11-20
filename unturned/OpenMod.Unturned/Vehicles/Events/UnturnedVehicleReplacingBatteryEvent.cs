using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;

namespace OpenMod.Unturned.Vehicles.Events
{
    public class UnturnedVehicleReplacingBatteryEvent : UnturnedVehicleEvent, ICancellableEvent
    {
        public UnturnedPlayer Instigator { get; }

        public byte BatteryQuality { get; }

        public bool IsCancelled { get; set; }

        public UnturnedVehicleReplacingBatteryEvent(UnturnedPlayer instigator, UnturnedVehicle vehicle, byte batteryQuality) : base(vehicle)
        {
            Instigator = instigator;
            BatteryQuality = batteryQuality;
        }
    }
}
