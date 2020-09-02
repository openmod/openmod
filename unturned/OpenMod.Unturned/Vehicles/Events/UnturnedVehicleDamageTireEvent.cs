using OpenMod.API.Eventing;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Vehicles.Events
{
    public class UnturnedVehicleDamageTireEvent : UnturnedVehicleEvent, ICancellableEvent
    {
        public CSteamID Instigator { get; }

        public int TireIndex { get; }

        public EDamageOrigin DamageOrigin { get; }

        public bool IsCancelled { get; set; }

        public UnturnedVehicleDamageTireEvent(InteractableVehicle vehicle, CSteamID instigator, int tireIndex, EDamageOrigin damageOrigin) : base(vehicle)
        {
            Instigator = instigator;
            TireIndex = tireIndex;
            DamageOrigin = damageOrigin;
        }
    }
}
