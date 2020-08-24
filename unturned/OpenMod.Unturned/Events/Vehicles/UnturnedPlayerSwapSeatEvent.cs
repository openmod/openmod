using OpenMod.API.Eventing;
using OpenMod.Unturned.Entities;
using SDG.Unturned;

namespace OpenMod.Unturned.Events.Vehicles
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