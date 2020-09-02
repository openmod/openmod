using OpenMod.Unturned.Players;

namespace OpenMod.Unturned.Zombies.Events
{
    public class UnturnedZombieAlertPlayerEvent : UnturnedZombieAlertEvent
    {
        public UnturnedPlayer Player { get; set; }

        public UnturnedZombieAlertPlayerEvent(UnturnedZombie zombie, UnturnedPlayer player) : base(zombie)
        {
            Player = player;
        }
    }
}
