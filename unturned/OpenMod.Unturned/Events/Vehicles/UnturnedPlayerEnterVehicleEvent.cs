using OpenMod.API.Eventing;
using OpenMod.Unturned.Entities;
using SDG.Unturned;

namespace OpenMod.Unturned.Events.Vehicles
{
    public class UnturnedPlayerEnterVehicleEvent : UnturnedPlayerEvent, ICancellableEvent
    {
        public InteractableVehicle Vehicle { get; }

        public bool IsCancelled { get; set; }

        public UnturnedPlayerEnterVehicleEvent(UnturnedPlayer player, InteractableVehicle vehicle) : base(player)
        {
            Vehicle = vehicle;
        }
    }
}
