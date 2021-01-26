using OpenMod.Unturned.Players;

namespace OpenMod.Unturned.Zombies.Events
{
    /// <summary>
    /// The event that is triggered when a zombie has been alerted to a player.
    /// </summary>
    public class UnturnedZombieAlertingPlayerEvent : UnturnedZombieAlertingEvent
    {
        /// <summary>
        /// The player alerting the zombie.
        /// </summary>
        public UnturnedPlayer Player { get; }

        public UnturnedZombieAlertingPlayerEvent(UnturnedZombie zombie, UnturnedPlayer player) : base(zombie)
        {
            Player = player;
        }
    }
}
