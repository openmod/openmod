using OpenMod.API.Eventing;
using OpenMod.Extensions.Games.Abstractions.Vehicles;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    public class PlayerExitVehicleEvent : PlayerEvent, ICancellableEvent
    {
        public IVehicle Vehicle { get; }

        public bool IsCancelled { get; set; }

        public PlayerExitVehicleEvent(IPlayer player, IVehicle vehicle) : base(player)
        {
            Vehicle = vehicle;
        }
    }
}