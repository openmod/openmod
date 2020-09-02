using OpenMod.API.Eventing;
using SDG.Unturned;
using UnityEngine;

namespace OpenMod.Unturned.Zombies.Events
{
    public class UnturnedZombieDamageEvent : UnturnedZombieEvent, ICancellableEvent
    {
        public ushort DamageAmount { get; set; }

        public Vector3 Ragdoll { get; set; }

        public ERagdollEffect RagdollEffect { get; set; }

        public bool TrackKill { get; set; }

        public bool DropLoot { get; set; }

        public EZombieStunOverride StunOverride { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedZombieDamageEvent(UnturnedZombie zombie, ushort damageAmount, Vector3 ragdoll, ERagdollEffect ragdollEffect, bool trackKill, bool dropLoot, EZombieStunOverride stunOverride) : base(zombie)
        {
            DamageAmount = damageAmount;
            Ragdoll = ragdoll;
            RagdollEffect = ragdollEffect;
            TrackKill = trackKill;
            DropLoot = dropLoot;
            StunOverride = stunOverride;
        }
    }
}
