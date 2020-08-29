using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Players.Stats
{
    public abstract class UnturnedPlayerStatUpdateEvent : UnturnedPlayerEvent
    {
        protected UnturnedPlayerBleedingUpdateEvent(UnturnedPlayer player) : base(player)
        {

        }
    }
}
