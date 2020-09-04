using OpenMod.API.Eventing;

namespace OpenMod.Unturned.Zombies.Events
{
    public abstract class UnturnedZombieAlertingEvent : UnturnedZombieEvent, ICancellableEvent
    {
        public bool IsCancelled { get; set; }

        protected UnturnedZombieAlertingEvent(UnturnedZombie zombie) : base(zombie)
        {
        }
    }
}
