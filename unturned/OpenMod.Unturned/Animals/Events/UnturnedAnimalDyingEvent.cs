using OpenMod.Unturned.Entities;
using SDG.Unturned;
using UnityEngine;

namespace OpenMod.Unturned.Animals.Events
{
    public class UnturnedAnimalDyingEvent : UnturnedAnimalDamageEvent
    {
        public UnturnedAnimalDyingEvent(UnturnedAnimal animal, ushort damageAmount, Vector3 ragdoll,
            ERagdollEffect ragdollEffect, bool trackKill, bool dropLoot) : base(animal, damageAmount, ragdoll,
            ragdollEffect, trackKill, dropLoot)
        {
        }
    }
}
