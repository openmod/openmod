using OpenMod.Unturned.Entities;
using SDG.Unturned;
using UnityEngine;

namespace OpenMod.Unturned.Events.Animals
{
    public class UnturnedAnimalDeadEvent : UnturnedAnimalEvent
    {
        public Vector3 Ragdoll { get; }

        public ERagdollEffect RagdollEffect { get; }

        public UnturnedAnimalDeadEvent(UnturnedAnimal animal, Vector3 ragdoll, ERagdollEffect ragdollEffect) : base(animal)
        {
            Ragdoll = ragdoll;
            RagdollEffect = ragdollEffect;
        }
    }
}
