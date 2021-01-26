namespace OpenMod.Unturned.Zombies.Events
{
    /// <summary>
    /// The event that is triggered when a new zombie is addded.
    /// </summary>
    public class UnturnedZombieAddedEvent : UnturnedZombieSpawnedEvent
    {
        public UnturnedZombieAddedEvent(UnturnedZombie zombie) : base(zombie)
        {

        }
    }
}
