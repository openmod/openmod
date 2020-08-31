using OpenMod.Unturned.Entities;
using SDG.Unturned;
using UnityEngine;

namespace OpenMod.Unturned.Events.Zombies
{
    public class UnturnedZombieDyingEvent : UnturnedZombieDamageEvent
    {
        public UnturnedZombieDyingEvent(UnturnedZombie zombie, ushort damageAmount, Vector3 ragdoll, ERagdollEffect ragdollEffect, bool trackKill, bool dropLoot, EZombieStunOverride stunOverride) : base(zombie, damageAmount, ragdoll, ragdollEffect, trackKill, dropLoot, stunOverride)
        {

        }
    }
}
