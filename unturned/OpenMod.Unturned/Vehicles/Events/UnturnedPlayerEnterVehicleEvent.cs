using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Extensions.Games.Abstractions.Vehicles;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Players;
using SDG.Unturned;

namespace OpenMod.Unturned.Vehicles.Events
{
    public class UnturnedPlayerEnterVehicleEvent : UnturnedPlayerEvent, IPlayerEnterVehicleEvent
    {
        public InteractableVehicle Vehicle { get; }

        IVehicle IVehicleEvent.Vehicle => new UnturnedVehicle(Vehicle);

        public bool IsCancelled { get; set; }

        public UnturnedPlayerEnterVehicleEvent(UnturnedPlayer player, InteractableVehicle vehicle) : base(player)
        {
            Vehicle = vehicle;
        }
    }
}
