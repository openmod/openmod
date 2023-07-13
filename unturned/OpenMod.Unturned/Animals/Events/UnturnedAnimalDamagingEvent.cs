using OpenMod.API.Eventing;
using SDG.Unturned;
using System.Numerics;
using Steamworks;

namespace OpenMod.Unturned.Animals.Events
{
    public class UnturnedAnimalDamagingEvent : UnturnedAnimalEvent, ICancellableEvent
    {
        public ushort DamageAmount { get; set; }

        public Vector3 Ragdoll { get; set; }

        public ERagdollEffect RagdollEffect { get; set; }
        
        public CSteamID Instigator { get; set; }

        public ELimb Limb { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedAnimalDamagingEvent(UnturnedAnimal animal, ushort damageAmount, Vector3 ragdoll,
            ERagdollEffect ragdollEffect, CSteamID instigator, ELimb limb) : base(animal)
        {
            DamageAmount = damageAmount;
            Ragdoll = ragdoll;
            RagdollEffect = ragdollEffect;
            Instigator = instigator;
            Limb = limb;
        }
    }
}