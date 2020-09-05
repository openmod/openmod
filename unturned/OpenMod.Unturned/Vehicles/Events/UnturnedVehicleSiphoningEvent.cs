using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;

namespace OpenMod.Unturned.Vehicles.Events
{
    public class UnturnedVehicleSiphoningEvent : UnturnedVehicleEvent, ICancellableEvent
    {
        public UnturnedPlayer Instigator { get; }

        public ushort DesiredAmount { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedVehicleSiphoningEvent(UnturnedVehicle vehicle, UnturnedPlayer instigator, ushort desiredAmount) : base(vehicle)
        {
            Instigator = instigator;
            DesiredAmount = desiredAmount;
        }
    }
}
