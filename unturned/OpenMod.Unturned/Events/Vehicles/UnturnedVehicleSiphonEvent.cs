using OpenMod.API.Eventing;
using OpenMod.Unturned.Entities;
using SDG.Unturned;

namespace OpenMod.Unturned.Events.Vehicles
{
    public class UnturnedVehicleSiphonEvent : UnturnedVehicleEvent, ICancellableEvent
    {
        public UnturnedPlayer Instigator { get; }

        public ushort DesiredAmount { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedVehicleSiphonEvent(InteractableVehicle vehicle, UnturnedPlayer instigator, ushort desiredAmount) : base(vehicle)
        {
            Instigator = instigator;
            DesiredAmount = desiredAmount;
        }
    }
}
