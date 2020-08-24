using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Players.Stats
{
    public class UnturnedPlayerVisionUpdateEvent : UnturnedPlayerEvent
    {
        public bool IsViewing { get; set; }

        public UnturnedPlayerVisionUpdateEvent(UnturnedPlayer player, bool isViewing) : base(player)
        {
            IsViewing = isViewing;
        }
    }
}
