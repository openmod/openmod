using OpenMod.Unturned.Events;

namespace OpenMod.Unturned.Players.Stats.Events
{
    public abstract class UnturnedPlayerStatUpdatedEvent : UnturnedPlayerEvent
    {
        protected UnturnedPlayerStatUpdatedEvent(UnturnedPlayer player) : base(player)
        {
           
        }
    }
}
