using OpenMod.API.Eventing;
using OpenMod.Unturned.Events;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Interacting.Events
{
    public class UnturnedPlayerBedClaimingEvent : UnturnedPlayerEvent, ICancellableEvent
    {
        public UnturnedPlayerBedClaimingEvent(InteractableBed bed, UnturnedPlayer player) : base(player)
        {
            Bed = bed;
        }

        public InteractableBed Bed { get; }
        public bool IsCancelled { get; set; }
    }
}