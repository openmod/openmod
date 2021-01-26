using SDG.Unturned;
using UnityEngine;

namespace OpenMod.Unturned.Zombies.Events
{
    /// <summary>
    /// The event that is triggered when a zombie has died.
    /// </summary>
    public class UnturnedZombieDeadEvent : UnturnedZombieEvent
    {
        /// <value>
        /// The ragdoll position.
        /// </value>
        public Vector3 Ragdoll { get; }

        /// <value>
        /// The ragdoll effect.
        /// </value>
        public ERagdollEffect RagdollEffect { get; }

        public UnturnedZombieDeadEvent(UnturnedZombie zombie, Vector3 ragdoll, ERagdollEffect ragdollEffect) : base(zombie)
        {
            Ragdoll = ragdoll;
            RagdollEffect = ragdollEffect;
        }
    }
}
