using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;
using Steamworks;

namespace OpenMod.Unturned.Vehicles.Events
{
    public class UnturnedVehicleLockUpdatingEvent : UnturnedVehicleEvent, ICancellableEvent
    {
        public UnturnedPlayer Instigator { get; }

        public CSteamID Group { get; }

        public bool IsLocked { get; }

        public bool IsCancelled { get; set; }

        public UnturnedVehicleLockUpdatingEvent(UnturnedPlayer instigator, UnturnedVehicle vehicle, CSteamID group, bool isLocked) : base(vehicle)
        {
            Instigator = instigator;
            Group = group;
            IsLocked = isLocked;
        }
    }
}
