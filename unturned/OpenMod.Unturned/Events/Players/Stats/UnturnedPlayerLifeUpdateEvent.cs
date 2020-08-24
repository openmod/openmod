using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Players.Stats
{
    public class UnturnedPlayerLifeUpdateEvent : UnturnedPlayerEvent
    {
        public bool IsDead { get; set; }

        public UnturnedPlayerLifeUpdateEvent(UnturnedPlayer player, bool isDead) : base(player)
        {
            IsDead = isDead;
        }
    }
}
