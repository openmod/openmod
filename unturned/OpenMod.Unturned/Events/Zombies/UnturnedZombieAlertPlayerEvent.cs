using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Zombies
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
