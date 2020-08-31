using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Animals
{
    public abstract class UnturnedAnimalAttackEvent : UnturnedAnimalAlertEvent
    {
        protected UnturnedAnimalAttackEvent(UnturnedAnimal animal, bool sendToPack) : base(animal, sendToPack)
        {
        }
    }
}
