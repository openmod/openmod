using OpenMod.Extensions.Games.Abstractions.Entities;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Unturned.Entities;
using SDG.Unturned;
using Steamworks;
using System;
using UnityEngine;

namespace OpenMod.Unturned.Events.Players.Damage
{
    public class UnturnedPlayerDamageEvent : UnturnedPlayerEvent, IPlayerDamageEvent
    {
        public byte DamageAmount { get; set; }

        double IPlayerDamageEvent.DamageAmount
        {
            get { return DamageAmount; }
            set { DamageAmount = (byte)Mathf.Clamp((float)Math.Ceiling(value), byte.MinValue, byte.MaxValue); }
        }

        public EDeathCause Cause { get; set; }

        public ELimb Limb { get; set; }

        public CSteamID Killer { get; set; }

        public IDamageSource DamageSource { get; }

        public bool TrackKill { get; set; }

        public Vector3 Ragdoll { get; set; }

        public ERagdollEffect RagdollEffect { get; set; }

        public bool CanCauseBleeding { get; set; }

        public bool IsCancelled { get; set; }
        
        public UnturnedPlayerDamageEvent(UnturnedPlayer player, byte amount, 
            EDeathCause cause, ELimb limb, 
            CSteamID killer, IDamageSource source,
            bool trackKill, 
            Vector3 ragdoll, ERagdollEffect ragdollEffect, 
            bool canCauseBleeding) : base(player)
        {
            DamageAmount = amount;
            Cause = cause;
            Limb = limb;
            Killer = killer;
            DamageSource = source;
            TrackKill = trackKill;
            Ragdoll = ragdoll;
            RagdollEffect = ragdollEffect;
            CanCauseBleeding = canCauseBleeding;
        }
    }
}
