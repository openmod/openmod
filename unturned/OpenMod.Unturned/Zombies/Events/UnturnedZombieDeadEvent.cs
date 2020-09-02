using OpenMod.Unturned.Entities;
using SDG.Unturned;
using UnityEngine;

namespace OpenMod.Unturned.Zombies.Events
{
    public class UnturnedZombieDeadEvent : UnturnedZombieEvent
    {
        public Vector3 Ragdoll { get; }

        public ERagdollEffect RagdollEffect { get; }

        public UnturnedZombieDeadEvent(UnturnedZombie zombie, Vector3 ragdoll, ERagdollEffect ragdollEffect) : base(zombie)
        {
            Ragdoll = ragdoll;
            RagdollEffect = ragdollEffect;
        }
    }
}
