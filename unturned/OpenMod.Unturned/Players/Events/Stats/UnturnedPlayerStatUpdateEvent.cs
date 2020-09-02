using OpenMod.Unturned.Events;

namespace OpenMod.Unturned.Players.Events.Stats
{
    public abstract class UnturnedPlayerStatUpdateEvent : UnturnedPlayerEvent
    {
        protected UnturnedPlayerStatUpdateEvent(UnturnedPlayer player) : base(player)
        {

        }
    }
}
