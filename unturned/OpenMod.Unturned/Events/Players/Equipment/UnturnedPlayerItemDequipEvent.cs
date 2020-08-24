using OpenMod.API.Eventing;
using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Players.Equipment
{
    public class UnturnedPlayerItemDequipEvent : UnturnedPlayerEvent, ICancellableEvent
    {
        public bool IsCancelled { get; set; }

        public UnturnedPlayerItemDequipEvent(UnturnedPlayer player) : base(player)
        {
            
        }
    }
}
