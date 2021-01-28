using OpenMod.Extensions.Games.Abstractions.Vehicles;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Players;

namespace OpenMod.Unturned.Vehicles.Events
{
    /// <summary>
    /// The event that is triggered when a player has swapped their seat.
    /// </summary>
    public class UnturnedPlayerSwappedSeatEvent : UnturnedPlayerEvent, IVehicleEvent
    {
        public UnturnedVehicle Vehicle { get; }

        IVehicle IVehicleEvent.Vehicle => Vehicle;

        /// <summary>
        /// Gets the seat the player has swapped from.
        /// </summary>
        public byte FromSeatIndex { get; }

        /// <summary>
        /// Gets the seat the player has swapped to.
        /// </summary>
        public byte ToSeatIndex { get; set; }

        public UnturnedPlayerSwappedSeatEvent(UnturnedPlayer player, UnturnedVehicle vehicle, byte fromSeatIndex, byte toSeatIndex) : base(player)
        {
            Vehicle = vehicle;
            FromSeatIndex = fromSeatIndex;
            ToSeatIndex = toSeatIndex;
        }
    }
}