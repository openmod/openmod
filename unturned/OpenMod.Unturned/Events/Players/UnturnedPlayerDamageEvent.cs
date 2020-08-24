using OpenMod.API.Eventing;
using OpenMod.Unturned.Entities;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace OpenMod.Unturned.Events.Players
{
    public class UnturnedPlayerDamageEvent : UnturnedPlayerEvent, ICancellableEvent
    {
        public byte DamageAmount { get; set; }

        public EDeathCause Cause { get; set; }

        public ELimb Limb { get; set; }

        public CSteamID Killer { get; set; }

        public bool TrackKill { get; set; }

        public Vector3 Ragdoll { get; set; }

        public ERagdollEffect RagdollEffect { get; set; }

        public bool CanCauseBleeding { get; set; }

        public bool IsCancelled { get; set; }
        
        public UnturnedPlayerDamageEvent(UnturnedPlayer player, byte amount, 
            EDeathCause cause, ELimb limb, 
            CSteamID killer, bool trackKill, 
            Vector3 ragdoll, ERagdollEffect ragdollEffect, 
            bool canCauseBleeding) : base(player)
        {
            DamageAmount = amount;
            Cause = cause;
            Limb = limb;
            Killer = killer;
            TrackKill = trackKill;
            Ragdoll = ragdoll;
            RagdollEffect = ragdollEffect;
            CanCauseBleeding = canCauseBleeding;
        }
    }
}
