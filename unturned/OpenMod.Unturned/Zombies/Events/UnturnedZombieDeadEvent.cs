using SDG.Unturned;
using UnityEngine;

namespace OpenMod.Unturned.Zombies.Events
{
    /// <summary>
    /// The event that is triggered when a zombie has died.
    /// </summary>
    public class UnturnedZombieDeadEvent : UnturnedZombieEvent
    {
        /// <summary>
        /// Gets the ragdoll position.
        /// </summary>
        public Vector3 Ragdoll { get; }

        /// <summary>
        /// Gets the ragdoll effect.
        /// </summary>
        public ERagdollEffect RagdollEffect { get; }

        public UnturnedZombieDeadEvent(UnturnedZombie zombie, Vector3 ragdoll, ERagdollEffect ragdollEffect) : base(zombie)
        {
            Ragdoll = ragdoll;
            RagdollEffect = ragdollEffect;
        }
    }
}
