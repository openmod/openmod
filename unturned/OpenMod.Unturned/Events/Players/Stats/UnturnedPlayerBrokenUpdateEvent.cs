using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Players.Stats
{
    public class UnturnedPlayerBrokenUpdateEvent : UnturnedPlayerEvent
    {
        public bool IsBroken { get; set; }

        public UnturnedPlayerBrokenUpdateEvent(UnturnedPlayer player, bool isBroken) : base(player)
        {
            IsBroken = isBroken;
        }
    }
}
