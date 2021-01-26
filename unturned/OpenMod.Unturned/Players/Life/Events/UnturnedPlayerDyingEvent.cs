using OpenMod.Extensions.Games.Abstractions.Entities;
using OpenMod.Extensions.Games.Abstractions.Players;
using SDG.Unturned;
using Steamworks;
using System.Numerics;

namespace OpenMod.Unturned.Players.Life.Events
{
    public class UnturnedPlayerDyingEvent : UnturnedPlayerDamagingEvent, IPlayerDyingEvent
    {
        public UnturnedPlayerDyingEvent(UnturnedPlayer player, byte amount,
            EDeathCause cause, ELimb limb,
            CSteamID killer, IDamageSource? source,
            bool trackKill,
            Vector3 ragdoll, ERagdollEffect ragdollEffect,
            bool canCauseBleeding) : base(player, amount, cause, limb,
                killer, source, trackKill, ragdoll, ragdollEffect, canCauseBleeding)
        {

        }
    }
}
