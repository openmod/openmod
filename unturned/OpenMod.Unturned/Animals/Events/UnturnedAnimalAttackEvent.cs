using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Animals.Events
{
    public abstract class UnturnedAnimalAttackEvent : UnturnedAnimalAlertEvent
    {
        protected UnturnedAnimalAttackEvent(UnturnedAnimal animal, bool sendToPack) : base(animal, sendToPack)
        {
        }
    }
}
