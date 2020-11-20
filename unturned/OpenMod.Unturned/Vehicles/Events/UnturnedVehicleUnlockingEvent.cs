using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;
using Steamworks;

namespace OpenMod.Unturned.Vehicles.Events
{
    public class UnturnedVehicleUnlockingEvent : UnturnedVehicleEvent, ICancellableEvent
    {
        public UnturnedPlayer Instigator { get; }

        public CSteamID Group { get; }

        public bool IsCancelled { get; set; }

        public UnturnedVehicleUnlockingEvent(UnturnedPlayer instigator, UnturnedVehicle vehicle, CSteamID group) : base(vehicle)
        {
            Instigator = instigator;
            Group = group;
        }
    }
}
