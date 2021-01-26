extern alias JetBrainsAnnotations;
using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;
using SDG.Unturned;
using UnityEngine;

namespace OpenMod.Unturned.Zombies.Events
{
    /// <summary>
    /// The event that is triggered when damage is dealt to a zombie.
    /// </summary>
    public class UnturnedZombieDamagingEvent : UnturnedZombieEvent, ICancellableEvent
    {
        /// <value>
        /// The amount of damage to deal.
        /// </value>
        public ushort DamageAmount { get; set; }

        /// <value>
        /// The ragdoll position.
        /// </value>
        public Vector3 Ragdoll { get; set; }

        /// <value>
        /// The player dealing the damage.
        /// </value>
        public UnturnedPlayer? Instigator { get; set; }

        /// <value>
        /// The ragdoll effect.
        /// </value>
        public ERagdollEffect RagdollEffect { get; set; }

        /// <value>
        /// The stun override.
        /// </value>
        public EZombieStunOverride StunOverride { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedZombieDamagingEvent(UnturnedZombie zombie, UnturnedPlayer? user, ushort damageAmount,
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
