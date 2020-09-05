using OpenMod.API.Eventing;

namespace OpenMod.Unturned.Animals.Events
{
    public abstract class UnturnedAnimalAlertingEvent : UnturnedAnimalEvent, ICancellableEvent
    {
        public bool SendToPack { get; set; }

        public bool IsCancelled { get; set; }

        protected UnturnedAnimalAlertingEvent(UnturnedAnimal animal, bool sendToPack) : base(animal)
        {
            SendToPack = sendToPack;
        }
    }
}
