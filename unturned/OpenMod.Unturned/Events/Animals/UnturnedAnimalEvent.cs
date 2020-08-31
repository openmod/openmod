using OpenMod.Core.Eventing;
using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Animals
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
