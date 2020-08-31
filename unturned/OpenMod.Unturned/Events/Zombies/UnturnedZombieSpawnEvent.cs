using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Zombies
{
    public abstract class UnturnedZombieSpawnEvent : UnturnedZombieEvent
    {
        protected UnturnedZombieSpawnEvent(UnturnedZombie zombie) : base(zombie)
        {

        }
    }
}
