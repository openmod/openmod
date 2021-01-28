using OpenMod.Unturned.Players;
using Steamworks;

namespace OpenMod.Unturned.Vehicles.Events
{
    /// <summary>
    /// The event that is triggered when the vehicle lock has been updated.
    /// </summary>
    public class UnturnedVehicleLockUpdatedEvent : UnturnedVehicleEvent
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
        /// Gets if the vehicle has been locked or unlocked.
        /// </summary>
        public bool IsLocked { get; }

        public UnturnedVehicleLockUpdatedEvent(UnturnedPlayer instigator, UnturnedVehicle vehicle, CSteamID? group, bool isLocked) : base(vehicle)
        {
            Instigator = instigator;
            Group = group;
            IsLocked = isLocked;
        }
    }
}
