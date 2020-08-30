using OpenMod.Unturned.Entities;
using SDG.Unturned;

namespace OpenMod.Unturned.Events.Zombies
{
    public class UnturnedZombieAlertPlayerEvent : UnturnedZombieAlertEvent
    {
        public UnturnedPlayer Player { get; set; }

        public UnturnedZombieAlertPlayerEvent(Zombie zombie, UnturnedPlayer player) : base(zombie)
        {
            Player = player;
        }
    }
}
