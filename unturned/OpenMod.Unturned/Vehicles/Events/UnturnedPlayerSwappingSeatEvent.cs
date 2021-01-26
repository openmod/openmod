using OpenMod.API.Eventing;
using OpenMod.Extensions.Games.Abstractions.Vehicles;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Players;

namespace OpenMod.Unturned.Vehicles.Events
{
    /// <summary>
    /// The event that is triggered when a player is swapping their seat.
    /// </summary>
    public class UnturnedPlayerSwappingSeatEvent : UnturnedPlayerEvent, ICancellableEvent, IVehicleEvent
    {
        public UnturnedVehicle Vehicle { get; }

        IVehicle IVehicleEvent.Vehicle => Vehicle;

        /// <value>
        /// The seat the player is swapping from.
        /// </value>
        public byte FromSeatIndex { get; }

        /// <value>
        /// The seat the player is swapping to.
        /// </value>
        public byte ToSeatIndex { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedPlayerSwappingSeatEvent(UnturnedPlayer player, UnturnedVehicle vehicle, byte fromSeatIndex, byte toSeatIndex) : base(player)
        {
            Vehicle = vehicle;
            FromSeatIndex = fromSeatIndex;
            ToSeatIndex = toSeatIndex;
        }
    }
}