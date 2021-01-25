using OpenMod.Unturned.Events;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Stats.Events
{
    public abstract class UnturnedPlayerStatUpdatedEvent : UnturnedPlayerEvent
    {
        protected UnturnedPlayerStatUpdatedEvent(UnturnedPlayer player) : base(player)
        {
           
        }
    }
}
