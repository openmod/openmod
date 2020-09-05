using OpenMod.API.Eventing;
using Steamworks;

namespace OpenMod.Unturned.Vehicles.Events
{
    public class UnturnedVehicleRepairingEvent : UnturnedVehicleEvent, ICancellableEvent
    {
        public CSteamID Instigator { get; }

        public ushort PendingTotalHealing { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedVehicleRepairingEvent(UnturnedVehicle vehicle, CSteamID instigator, ushort pendingTotalHealing) : base(vehicle)
        {
            Instigator = instigator;
            PendingTotalHealing = pendingTotalHealing;
        }
    }
}
