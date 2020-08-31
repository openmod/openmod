using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Animals
{
    public class UnturnedAnimalReviveEvent : UnturnedAnimalSpawnEvent
    {
        public UnturnedAnimalReviveEvent(UnturnedAnimal animal) : base(animal)
        {
        }
    }
}
