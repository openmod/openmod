using System.Numerics;

namespace OpenMod.Unturned.Animals.Events
{
    public class UnturnedAnimalFleeingEvent : UnturnedAnimalAlertingEvent
    {
        public Vector3 Direction { get; set; }

        public UnturnedAnimalFleeingEvent(UnturnedAnimal animal, Vector3 direction, bool sendToPack) : base(animal, sendToPack)
        {
            Direction = direction;
        }
    }
}
