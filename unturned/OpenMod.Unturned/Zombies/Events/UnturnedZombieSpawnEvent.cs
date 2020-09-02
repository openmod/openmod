using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Zombies.Events
{
    public abstract class UnturnedZombieSpawnEvent : UnturnedZombieEvent
    {
        protected UnturnedZombieSpawnEvent(UnturnedZombie zombie) : base(zombie)
        {

        }
    }
}
