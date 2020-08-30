using SDG.Unturned;

namespace OpenMod.Unturned.Events.Zombies
{
    public abstract class UnturnedZombieSpawnEvent : UnturnedZombieEvent
    {
        protected UnturnedZombieSpawnEvent(Zombie zombie) : base(zombie)
        {

        }
    }
}
