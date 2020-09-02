using OpenMod.API.Eventing;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Players;
using SDG.Unturned;

namespace OpenMod.Unturned.Vehicles.Events
{
    public class UnturnedPlayerSwapSeatEvent : UnturnedPlayerEvent, ICancellableEvent
    {
        public InteractableVehicle Vehicle { get; }

        public byte FromSeatIndex { get; }

        public byte ToSeatIndex { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedPlayerSwapSeatEvent(UnturnedPlayer player, InteractableVehicle vehicle, byte fromSeatIndex, byte toSeatIndex) : base(player)
        {
            Vehicle = vehicle;
            FromSeatIndex = fromSeatIndex;
            ToSeatIndex = toSeatIndex;
        }
    }
}