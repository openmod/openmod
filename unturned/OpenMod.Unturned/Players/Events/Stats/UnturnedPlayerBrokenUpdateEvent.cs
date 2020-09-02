using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Players.Events.Stats
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
