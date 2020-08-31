using OpenMod.API.Eventing;
using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Animals
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
