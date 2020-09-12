namespace OpenMod.Unturned.Players.Stats.Events
{
    public class UnturnedPlayerLifeUpdatedEvent : UnturnedPlayerStatUpdatedEvent
    {
        public bool IsDead { get; }

        public UnturnedPlayerLifeUpdatedEvent(UnturnedPlayer player, bool isDead) : base(player)
        {
            IsDead = isDead;
        }
    }
}
