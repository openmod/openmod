using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Vehicles.Events
{
    public class UnturnedVehicleStealBatteryEvent : UnturnedVehicleEvent, ICancellableEvent
    {
        public UnturnedPlayer Instigator { get; }

        public bool IsCancelled { get; set; }

        public UnturnedVehicleStealBatteryEvent(UnturnedPlayer instigator, UnturnedVehicle vehicle) : base(vehicle)
        {
            Instigator = instigator;
        }
    }
}
