using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Zombies
{
    public class UnturnedZombieAddedEvent : UnturnedZombieSpawnEvent
    {
        public UnturnedZombieAddedEvent(UnturnedZombie zombie) : base(zombie)
        {

        }
    }
}
