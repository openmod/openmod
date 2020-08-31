using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Animals
{
    public class UnturnedAnimalAddEvent : UnturnedAnimalSpawnEvent
    {
        public UnturnedAnimalAddEvent(UnturnedAnimal animal) : base(animal)
        {
        }
    }
}
