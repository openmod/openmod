using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;
using Steamworks;

namespace OpenMod.Unturned.Vehicles.Events
{
    public class UnturnedVehicleLockingEvent : UnturnedVehicleEvent, ICancellableEvent
    {
        public UnturnedPlayer Instigator { get; }

        public CSteamID Group { get; }

        public bool IsCancelled { get; set; }

        public UnturnedVehicleLockingEvent(UnturnedPlayer instigator, UnturnedVehicle vehicle, CSteamID group) : base(vehicle)
        {
            Instigator = instigator;
            Group = group;
        }
    }
}
