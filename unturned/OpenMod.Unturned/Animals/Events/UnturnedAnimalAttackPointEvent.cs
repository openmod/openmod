using System.Numerics;

namespace OpenMod.Unturned.Animals.Events
{
    public class UnturnedAnimalAttackPointEvent : UnturnedAnimalAttackEvent
    {
        public Vector3 Point { get; set; }

        public UnturnedAnimalAttackPointEvent(UnturnedAnimal animal, Vector3 point, bool sendToPack) : base(animal, sendToPack)
        {
            Point = point;
        }
    }
}
