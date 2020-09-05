using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Extensions.Games.Abstractions.Vehicles;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Players;

namespace OpenMod.Unturned.Vehicles.Events
{
    public class UnturnedPlayerEnteringVehicleEvent : UnturnedPlayerEvent, IPlayerEnteringVehicleEvent
    {
        public UnturnedVehicle Vehicle { get; }

        IVehicle IVehicleEvent.Vehicle => Vehicle;

        public bool IsCancelled { get; set; }

        public UnturnedPlayerEnteringVehicleEvent(UnturnedPlayer player, UnturnedVehicle vehicle) : base(player)
        {
            Vehicle = vehicle;
        }
    }
}
