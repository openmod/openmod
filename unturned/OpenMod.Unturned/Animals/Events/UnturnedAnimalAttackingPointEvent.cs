using System.Numerics;

namespace OpenMod.Unturned.Animals.Events
{
    public class UnturnedAnimalAttackingPointEvent : UnturnedAnimalAttackingEvent
    {
        public Vector3 Point { get; set; }

        public UnturnedAnimalAttackingPointEvent(UnturnedAnimal animal, Vector3 point, bool sendToPack) : base(animal, sendToPack)
        {
            Point = point;
        }
    }
}
