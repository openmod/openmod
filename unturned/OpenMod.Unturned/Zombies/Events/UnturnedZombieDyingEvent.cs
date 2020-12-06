using OpenMod.Unturned.Players;
using SDG.Unturned;
using UnityEngine;

namespace OpenMod.Unturned.Zombies.Events
{
    public class UnturnedZombieDyingEvent : UnturnedZombieDamagingEvent
    {
        public UnturnedZombieDyingEvent(UnturnedZombie zombie, UnturnedPlayer player, ushort damageAmount, Vector3 ragdoll,
            ERagdollEffect ragdollEffect, bool trackKill, bool dropLoot, EZombieStunOverride stunOverride)
                : base(zombie, player, damageAmount, ragdoll, ragdollEffect, trackKill, dropLoot, stunOverride)
        {

        }
    }
}
