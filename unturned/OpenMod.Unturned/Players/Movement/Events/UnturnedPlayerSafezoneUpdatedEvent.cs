using OpenMod.Unturned.Events;

namespace OpenMod.Unturned.Players.Movement.Events
{
    public class UnturnedPlayerSafezoneUpdatedEvent : UnturnedPlayerEvent
    {
        public bool IsSafe { get; }
        public UnturnedPlayerSafezoneUpdatedEvent(UnturnedPlayer player, bool safe) : base(player)
        {
            IsSafe = safe;
        }
    }
}
