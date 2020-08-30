using OpenMod.API.Eventing;
using SDG.Unturned;

namespace OpenMod.Unturned.Events.Zombies
{
    public abstract class UnturnedZombieAlertEvent : UnturnedZombieEvent, ICancellableEvent
    {
        public bool IsCancelled { get; set; }

        protected UnturnedZombieAlertEvent(Zombie zombie) : base(zombie)
        {
        }
    }
}
