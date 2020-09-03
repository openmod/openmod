using SDG.Unturned;
using System.Numerics;

namespace OpenMod.Unturned.Animals.Events
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
