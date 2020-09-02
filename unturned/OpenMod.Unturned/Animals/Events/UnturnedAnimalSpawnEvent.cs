using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Animals.Events
{
    public abstract class UnturnedAnimalSpawnEvent : UnturnedAnimalEvent
    {
        protected UnturnedAnimalSpawnEvent(UnturnedAnimal animal) : base(animal)
        {
        }
    }
}
