using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Extensions.Games.Abstractions.Vehicles;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Players;

namespace OpenMod.Unturned.Vehicles.Events
{
    public class UnturnedPlayerEnteredVehicleEvent : UnturnedPlayerEvent, IPlayerEnteredVehicleEvent
    {
        public UnturnedVehicle Vehicle { get; }

        IVehicle IVehicleEvent.Vehicle => Vehicle;

        public UnturnedPlayerEnteredVehicleEvent(UnturnedPlayer player, UnturnedVehicle vehicle) : base(player)
        {
            Vehicle = vehicle;
        }
    }
}
