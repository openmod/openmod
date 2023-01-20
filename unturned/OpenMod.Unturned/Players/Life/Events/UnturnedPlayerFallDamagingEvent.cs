using OpenMod.Extensions.Games.Abstractions.Entities;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Unturned.Events;
using System;
using UnityEngine;

namespace OpenMod.Unturned.Players.Life.Events
{
    public class UnturnedPlayerFallDamagingEvent : UnturnedPlayerEvent, IPlayerDamagingEvent
    {
        public float Velocity { get; }

        public byte DamageAmount { get; set; }

        public IDamageSource? DamageSource { get; }
        double IPlayerDamagingEvent.DamageAmount
        {
            get { return DamageAmount; }
            set { DamageAmount = (byte)Mathf.Clamp((float)Math.Ceiling(value), byte.MinValue, byte.MaxValue); }
        }

        public bool ShouldBreakLegs { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedPlayerFallDamagingEvent(UnturnedPlayer player, float velocity, byte amount, IDamageSource? source, bool shouldBreakLegs) : base(player)
        {
            Velocity = velocity;
            DamageAmount = amount;
            DamageSource = source;
            ShouldBreakLegs = shouldBreakLegs;
        }
    }
}
