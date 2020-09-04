namespace OpenMod.Unturned.Animals.Events
{
    public abstract class UnturnedAnimalSpawnedEvent : UnturnedAnimalEvent
    {
        protected UnturnedAnimalSpawnedEvent(UnturnedAnimal animal) : base(animal)
        {
        }
    }
}
