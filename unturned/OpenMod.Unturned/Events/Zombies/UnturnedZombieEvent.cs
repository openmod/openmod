using OpenMod.Core.Eventing;
using SDG.Unturned;

namespace OpenMod.Unturned.Events.Zombies
{
    public abstract class UnturnedZombieEvent : Event
    {
        protected UnturnedZombieEvent(Zombie zombie)
        {
            Zombie = zombie;
        }

        public Zombie Zombie { get; }
    }
}
