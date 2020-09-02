using OpenMod.API.Eventing;

namespace OpenMod.Unturned.Animals.Events
{
    public abstract class UnturnedAnimalAlertEvent : UnturnedAnimalEvent, ICancellableEvent
    {
        public bool SendToPack { get; set; }

        public bool IsCancelled { get; set; }

        protected UnturnedAnimalAlertEvent(UnturnedAnimal animal, bool sendToPack) : base(animal)
        {
            SendToPack = sendToPack;
        }
    }
}
