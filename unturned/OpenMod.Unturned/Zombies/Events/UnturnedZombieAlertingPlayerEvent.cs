using OpenMod.Unturned.Players;

namespace OpenMod.Unturned.Zombies.Events
{
    public class UnturnedZombieAlertingPlayerEvent : UnturnedZombieAlertingEvent
    {
        public UnturnedPlayer Player { get; set; }

        public UnturnedZombieAlertingPlayerEvent(UnturnedZombie zombie, UnturnedPlayer player) : base(zombie)
        {
            Player = player;
        }
    }
}
