using OpenMod.Core.Eventing;

namespace OpenMod.Unturned.Zombies.Events
{
    /// <summary>
    /// The base class for all zombie related events.
    /// </summary>
    public abstract class UnturnedZombieEvent : Event
    {
        protected UnturnedZombieEvent(UnturnedZombie zombie)
        {
            Zombie = zombie;
        }

        public UnturnedZombie Zombie { get; }
    }
}
