using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Players.Stats
{
    public class UnturnedPlayerVisionUpdateEvent : UnturnedPlayerStatUpdateEvent
    {
        public bool IsViewing { get; }

        public UnturnedPlayerVisionUpdateEvent(UnturnedPlayer player, bool isViewing) : base(player)
        {
            IsViewing = isViewing;
        }
    }
}
