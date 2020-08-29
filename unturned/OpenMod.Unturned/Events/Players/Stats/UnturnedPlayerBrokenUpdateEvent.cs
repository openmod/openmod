using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Players.Stats
{
    public class UnturnedPlayerBrokenUpdateEvent : UnturnedPlayerStatUpdateEvent
    {
        public bool IsBroken { get; }

        public UnturnedPlayerBrokenUpdateEvent(UnturnedPlayer player, bool isBroken) : base(player)
        {
            IsBroken = isBroken;
        }
    }
}
