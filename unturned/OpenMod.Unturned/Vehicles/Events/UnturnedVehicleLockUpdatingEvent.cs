using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;
using Steamworks;

namespace OpenMod.Unturned.Vehicles.Events
{
    /// <summary>
    /// The event that is triggered when a vehicle lock status is getting updated.
    /// </summary>
    public class UnturnedVehicleLockUpdatingEvent : UnturnedVehicleEvent, ICancellableEvent
    {
        /// <summary>
        /// Gets the player locking or unlocking the vehicle.
        /// </summary>
        public UnturnedPlayer Instigator { get; }

        /// <summary>
        /// Gets the new lock owner group.
        /// </summary>
        public CSteamID? Group { get; }

        /// <summary>
        /// Gets if the vehicle is to be locked or unlocked.
        /// </summary>
        public bool IsLocking { get; }

        public bool IsCancelled { get; set; }

        public UnturnedVehicleLockUpdatingEvent(UnturnedPlayer instigator, UnturnedVehicle vehicle, CSteamID? group, bool isLocking) : base(vehicle)
        {
            Instigator = instigator;
            Group = group;
            IsLocking = isLocking;
        }
    }
}
