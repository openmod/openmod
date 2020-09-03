using OpenMod.API.Eventing;
using OpenMod.Extensions.Games.Abstractions.Vehicles;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Players;

namespace OpenMod.Unturned.Vehicles.Events
{
    public class UnturnedPlayerSwapSeatEvent : UnturnedPlayerEvent, ICancellableEvent, IVehicleEvent
    {
        public UnturnedVehicle Vehicle { get; }

        IVehicle IVehicleEvent.Vehicle => Vehicle;

        public byte FromSeatIndex { get; }

        public byte ToSeatIndex { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedPlayerSwapSeatEvent(UnturnedPlayer player, UnturnedVehicle vehicle, byte fromSeatIndex, byte toSeatIndex) : base(player)
        {
            Vehicle = vehicle;
            FromSeatIndex = fromSeatIndex;
            ToSeatIndex = toSeatIndex;
        }
    }
}