using OpenMod.Unturned.Events;

namespace OpenMod.Unturned.Players.Events.Stats
{
    public abstract class UnturnedPlayerStatUpdatedEvent : UnturnedPlayerEvent
    {
        protected UnturnedPlayerStatUpdatedEvent(UnturnedPlayer player) : base(player)
        {

        }
    }
}
