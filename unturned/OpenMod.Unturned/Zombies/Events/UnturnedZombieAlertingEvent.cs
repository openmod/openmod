using OpenMod.API.Eventing;

namespace OpenMod.Unturned.Zombies.Events
{
    /// <summary>
    /// The base class for all zombie alert related events.
    /// </summary>
    public abstract class UnturnedZombieAlertingEvent : UnturnedZombieEvent, ICancellableEvent
    {
        public bool IsCancelled { get; set; }

        protected UnturnedZombieAlertingEvent(UnturnedZombie zombie) : base(zombie)
        {
        }
    }
}
