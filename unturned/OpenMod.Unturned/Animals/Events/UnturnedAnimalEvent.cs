using OpenMod.Core.Eventing;

namespace OpenMod.Unturned.Animals.Events
{
    public abstract class UnturnedAnimalEvent : Event
    {
        public UnturnedAnimal Animal { get; }

        protected UnturnedAnimalEvent(UnturnedAnimal animal)
        {
            Animal = animal;
        }
    }
}
