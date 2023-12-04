using OpenMod.API.Eventing;
using OpenMod.Unturned.Events;

namespace OpenMod.Unturned.Players.Quests.Events
{
    public class UnturnedPlayerLeavingGroupEvent : UnturnedPlayerEvent, ICancellableEvent
    {
        public bool IsCancelled { get; set; }

        public UnturnedPlayerLeavingGroupEvent(UnturnedPlayer player) : base(player)
        {

        }
    }
}
