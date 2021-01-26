using OpenMod.Extensions.Games.Abstractions.Entities;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using Steamworks;
using System;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace OpenMod.Unturned.Players.Life.Events
{
    public class UnturnedPlayerDamagingEvent : UnturnedPlayerEvent, IPlayerDamagingEvent
    {
        public byte DamageAmount { get; set; }

        double IPlayerDamagingEvent.DamageAmount
        {
            get { return DamageAmount; }
            set { DamageAmount = (byte)Mathf.Clamp((float)Math.Ceiling(value), byte.MinValue, byte.MaxValue); }
        }

        public EDeathCause Cause { get; set; }

        public ELimb Limb { get; set; }

        public CSteamID Killer { get; set; }

        public IDamageSource? DamageSource { get; }

        public bool TrackKill { get; set; }

        public Vector3 Ragdoll { get; set; }

        public ERagdollEffect RagdollEffect { get; set; }

        public bool CanCauseBleeding { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedPlayerDamagingEvent(UnturnedPlayer player, byte amount,
            EDeathCause cause, ELimb limb,
            CSteamID killer, IDamageSource? source,
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
