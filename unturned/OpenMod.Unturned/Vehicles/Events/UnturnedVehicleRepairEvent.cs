using OpenMod.API.Eventing;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Vehicles.Events
{
    public class UnturnedVehicleRepairEvent : UnturnedVehicleEvent, ICancellableEvent
    {
        public CSteamID Instigator { get; }

        public ushort PendingTotalHealing { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedVehicleRepairEvent(InteractableVehicle vehicle, CSteamID instigator, ushort pendingTotalHealing) : base(vehicle)
        {
            Instigator = instigator;
            PendingTotalHealing = pendingTotalHealing;
        }
    }
}
