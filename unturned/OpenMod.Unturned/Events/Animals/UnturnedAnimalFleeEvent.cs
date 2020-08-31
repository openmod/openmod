using OpenMod.Unturned.Entities;
using UnityEngine;

namespace OpenMod.Unturned.Events.Animals
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
