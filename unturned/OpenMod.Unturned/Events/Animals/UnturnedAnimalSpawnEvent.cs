using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Animals
{
    public abstract class UnturnedAnimalSpawnEvent : UnturnedAnimalEvent
    {
        protected UnturnedAnimalSpawnEvent(UnturnedAnimal animal) : base(animal)
        {
        }
    }
}
