using OpenMod.API.Eventing;
using SDG.Unturned;
using System.Numerics;

namespace OpenMod.Unturned.Animals.Events
{
    public class UnturnedAnimalDamagingEvent : UnturnedAnimalEvent, ICancellableEvent
    {
        public ushort DamageAmount { get; set; }

        public Vector3 Ragdoll { get; set; }

        public ERagdollEffect RagdollEffect { get; set; }

        public bool TrackKill { get; set; }

        public bool DropLoot { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedAnimalDamagingEvent(UnturnedAnimal animal, ushort damageAmount, Vector3 ragdoll, ERagdollEffect ragdollEffect, bool trackKill, bool dropLoot) : base(animal)
        {
            DamageAmount = damageAmount;
            Ragdoll = ragdoll;
            RagdollEffect = ragdollEffect;
            TrackKill = trackKill;
            DropLoot = dropLoot;
        }
    }
}
