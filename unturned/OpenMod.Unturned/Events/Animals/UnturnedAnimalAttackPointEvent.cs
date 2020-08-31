using OpenMod.Unturned.Entities;
using UnityEngine;

namespace OpenMod.Unturned.Events.Animals
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
