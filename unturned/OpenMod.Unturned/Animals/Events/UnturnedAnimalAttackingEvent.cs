namespace OpenMod.Unturned.Animals.Events
{
    public abstract class UnturnedAnimalAttackingEvent : UnturnedAnimalAlertingEvent
    {
        protected UnturnedAnimalAttackingEvent(UnturnedAnimal animal, bool sendToPack) : base(animal, sendToPack)
        {
        }
    }
}
