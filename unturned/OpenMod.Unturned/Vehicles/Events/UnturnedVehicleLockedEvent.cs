using OpenMod.Unturned.Players;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Vehicles.Events
{
    public class UnturnedVehicleLockedEvent : UnturnedVehicleEvent
    {
        public UnturnedPlayer Instigator { get; }

        public CSteamID Group { get; }

        public UnturnedVehicleLockedEvent(UnturnedPlayer instigator, UnturnedVehicle vehicle, CSteamID group) : base(vehicle)
        {
            Instigator = instigator;
            Group = group;
        }
    }
}
