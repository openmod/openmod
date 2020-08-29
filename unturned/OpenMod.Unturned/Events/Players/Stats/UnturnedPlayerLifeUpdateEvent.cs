using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Players.Stats
{
    public class UnturnedPlayerLifeUpdateEvent : UnturnedPlayerStatUpdateEvent
    {
        public bool IsDead { get; }

        public UnturnedPlayerLifeUpdateEvent(UnturnedPlayer player, bool isDead) : base(player)
        {
            IsDead = isDead;
        }
    }
}
