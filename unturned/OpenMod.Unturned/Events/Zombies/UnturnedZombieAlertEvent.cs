using OpenMod.API.Eventing;
using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Zombies
{
    public abstract class UnturnedZombieAlertEvent : UnturnedZombieEvent, ICancellableEvent
    {
        public bool IsCancelled { get; set; }

        protected UnturnedZombieAlertEvent(UnturnedZombie zombie) : base(zombie)
        {
        }
    }
}
