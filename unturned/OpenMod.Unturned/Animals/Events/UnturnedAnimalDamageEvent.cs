using OpenMod.API.Eventing;
using OpenMod.Unturned.Entities;
using SDG.Unturned;
using UnityEngine;

namespace OpenMod.Unturned.Animals.Events
{
    public class UnturnedAnimalDamageEvent : UnturnedAnimalEvent, ICancellableEvent
    {
        public ushort DamageAmount { get; set; }

        public Vector3 Ragdoll { get; set; }

        public ERagdollEffect RagdollEffect { get; set; }

        public bool TrackKill { get; set; }

        public bool DropLoot { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedAnimalDamageEvent(UnturnedAnimal animal, ushort damageAmount, Vector3 ragdoll, ERagdollEffect ragdollEffect, bool trackKill, bool dropLoot) : base(animal)
        {
            DamageAmount = damageAmount;
            Ragdoll = ragdoll;
            RagdollEffect = ragdollEffect;
            TrackKill = trackKill;
            DropLoot = dropLoot;
        }
    }
}
