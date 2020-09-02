namespace OpenMod.Unturned.Players.Events.Stats
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
