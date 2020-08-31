using OpenMod.Core.Eventing;
using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Zombies
{
    public abstract class UnturnedZombieEvent : Event
    {
        protected UnturnedZombieEvent(UnturnedZombie zombie)
        {
            Zombie = zombie;
        }

        public UnturnedZombie Zombie { get; }
    }
}
