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
        /// <value>
        /// The player updating the lock status.
        /// </value>
        public UnturnedPlayer Instigator { get; }

        /// <value>
        /// The new lock owner group.
        /// </value>
        public CSteamID? Group { get; }

        /// <value>
        /// <b>True</b> if the vehicle is getting locked; otherwise, if it is getting unlocked, <b>false</b>.
        /// </value>
        public bool IsLocked { get; }

        public bool IsCancelled { get; set; }

        public UnturnedVehicleLockUpdatingEvent(UnturnedPlayer instigator, UnturnedVehicle vehicle, CSteamID? group, bool isLocked) : base(vehicle)
        {
            Instigator = instigator;
            Group = group;
            IsLocked = isLocked;
        }
    }
}
