namespace OpenMod.Unturned.Zombies.Events
{
    /// <summary>
    /// The event that is triggered when a zombie has been revived.
    /// </summary>
    public class UnturnedZombieRevivedEvent : UnturnedZombieSpawnedEvent
    {
        public UnturnedZombieRevivedEvent(UnturnedZombie zombie) : base(zombie)
        {

        }
    }
}
