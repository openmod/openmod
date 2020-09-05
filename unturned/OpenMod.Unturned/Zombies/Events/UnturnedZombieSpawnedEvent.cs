namespace OpenMod.Unturned.Zombies.Events
{
    public abstract class UnturnedZombieSpawnedEvent : UnturnedZombieEvent
    {
        protected UnturnedZombieSpawnedEvent(UnturnedZombie zombie) : base(zombie)
        {

        }
    }
}
