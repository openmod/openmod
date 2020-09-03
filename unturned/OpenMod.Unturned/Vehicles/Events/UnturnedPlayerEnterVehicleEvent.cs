using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Extensions.Games.Abstractions.Vehicles;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Players;

namespace OpenMod.Unturned.Vehicles.Events
{
    public class UnturnedPlayerEnterVehicleEvent : UnturnedPlayerEvent, IPlayerEnterVehicleEvent
    {
        public UnturnedVehicle Vehicle { get; }

        IVehicle IVehicleEvent.Vehicle => Vehicle;

        public bool IsCancelled { get; set; }

        public UnturnedPlayerEnterVehicleEvent(UnturnedPlayer player, UnturnedVehicle vehicle) : base(player)
        {
            Vehicle = vehicle;
        }
    }
}
