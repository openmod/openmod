using SDG.Unturned;
using System.Numerics;

namespace OpenMod.Unturned.Animals.Events
{
    public class UnturnedAnimalDyingEvent : UnturnedAnimalDamagingEvent
    {
        public UnturnedAnimalDyingEvent(UnturnedAnimal animal, ushort damageAmount, Vector3 ragdoll,
            ERagdollEffect ragdollEffect, bool trackKill, bool dropLoot) : base(animal, damageAmount, ragdoll,
            ragdollEffect, trackKill, dropLoot)
        {
        }
    }
}
