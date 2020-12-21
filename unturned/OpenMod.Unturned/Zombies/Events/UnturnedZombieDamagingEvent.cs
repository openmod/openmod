using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;
using SDG.Unturned;
using UnityEngine;

namespace OpenMod.Unturned.Zombies.Events
{
    public class UnturnedZombieDamagingEvent : UnturnedZombieEvent, ICancellableEvent
    {
        public ushort DamageAmount { get; set; }

        public Vector3 Ragdoll { get; set; }

        public UnturnedPlayer Instigator { get; set; }

        public ERagdollEffect RagdollEffect { get; set; }

        public EZombieStunOverride StunOverride { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedZombieDamagingEvent(UnturnedZombie zombie, UnturnedPlayer user, ushort damageAmount,
            Vector3 ragdoll, ERagdollEffect ragdollEffect, EZombieStunOverride stunOverride) : base(zombie)
        {
            DamageAmount = damageAmount;
            Instigator = user;
            Ragdoll = ragdoll;
            RagdollEffect = ragdollEffect;
            StunOverride = stunOverride;
        }
    }
}
