using UnityEngine;

namespace OpenMod.Unturned.Animals.Events
{
    public class UnturnedAnimalFleeEvent : UnturnedAnimalAlertEvent
    {
        public Vector3 Direction { get; set; }

        public UnturnedAnimalFleeEvent(UnturnedAnimal animal, Vector3 direction, bool sendToPack) : base(animal, sendToPack)
        {
            Direction = direction;
        }
    }
}
