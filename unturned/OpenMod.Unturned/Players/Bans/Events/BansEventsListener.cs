extern alias JetBrainsAnnotations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using HarmonyLib;
using JetBrainsAnnotations::JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Nito.AsyncEx;
using OpenMod.API.Users;
using OpenMod.Core.Users;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Patching;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Players.Bans.Events
{
    [UsedImplicitly]
    internal class BansEventsListener : UnturnedEventsListener
    {
        private readonly IUserDataStore m_UserDataStore;

        public BansEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            m_UserDataStore = serviceProvider.GetRequiredService<IUserDataStore>();
        }

        public override void Subscribe()
        {
            Provider.onBanPlayerRequestedV2 += OnBanPlayerRequested;
            Provider.onCheckBanStatusWithHWID += OnCheckBanStatus;
            Provider.onUnbanPlayerRequested += OnUnbanPlayerRequested;
            OnBanned += Events_OnBanned;
            OnUnbanned += Events_OnUnbanned;
        }

        public override void Unsubscribe()
        {
            Provider.onBanPlayerRequestedV2 -= OnBanPlayerRequested;
            Provider.onCheckBanStatusWithHWID -= OnCheckBanStatus;
            Provider.onUnbanPlayerRequested -= OnUnbanPlayerRequested;

            OnBanned -= Events_OnBanned;
            OnUnbanned -= Events_OnUnbanned;
        }

        private void OnBanPlayerRequested(CSteamID instigator, CSteamID playerToBan, uint ipToBan, IEnumerable<byte[]> hwidsToBan, ref string reason, ref uint duration, ref bool shouldVanillaBan) // lgtm [cs/too-many-ref-parameters]
        {
            var player = GetUnturnedPlayer(PlayerTool.getPlayer(playerToBan))!;
            var ipAddressToBan = IPAddress.Parse(ipToBan.ToString());

            var @event = new UnturnedPlayerBanningEvent(
                player,
                instigator.ToString(),
                KnownActorTypes.Player,
                Equals(ipAddressToBan, IPAddress.Any) ? null : ipAddressToBan,
                hwidsToBan,
                reason,
                TimeSpan.FromSeconds(duration))
            {
                IsCancelled = !shouldVanillaBan
            };

            Emit(@event);

            reason = @event.Reason ?? "";
            duration = (uint)@event.Duration.TotalSeconds;
            shouldVanillaBan = !@event.IsCancelled;
        }

        private void OnCheckBanStatus(SteamPlayerID playerId, uint remoteIp, ref bool isBanned, ref string banReason, ref uint banRemainingDuration) // lgtm [cs/too-many-ref-parameters]
        {
            if (!isBanned)
            {
                var banned = isBanned;
                var duration = banRemainingDuration;
                var reason = banReason;

                AsyncContext.Run(async () =>
                {
                    var userData = await m_UserDataStore.GetUserDataAsync(playerId.steamID.ToString(), KnownActorTypes.Player);
                    if (userData?.BanInfo?.ExpireDate == null)
                    {
                        return;
                    }

                    var dur = (userData.BanInfo.ExpireDate.Value - DateTime.Now).TotalSeconds;
                    if (dur < 0)
                    {
                        return;
                    }

                    banned = true;
                    duration = dur > uint.MaxValue ? SteamBlacklist.PERMANENT : (uint)dur;
                    reason = userData.BanInfo.Reason;
                });

                isBanned = banned;
                banRemainingDuration = duration;
                banReason = reason;
            }


            var @event =
                new UnturnedPlayerCheckingBanEvent(playerId, remoteIp, isBanned, banReason, banRemainingDuration);

            Emit(@event);

            isBanned = @event.IsBanned;
            banReason = @event.Reason;
            banRemainingDuration = @event.RemainingDuration;
        }

        private void OnUnbanPlayerRequested(CSteamID instigator, CSteamID playerToUnban, ref bool shouldVanillaUnban)
        {
            var @event = new UnturnedPlayerUnbanningEvent(instigator, playerToUnban)
            {
                IsCancelled = !shouldVanillaUnban
            };

            Emit(@event);

            shouldVanillaUnban = !@event.IsCancelled;
        }

        private void Events_OnBanned(CSteamID instigator, CSteamID bannedPlayer, uint ip, string reason, uint duration)
        {
            var @event = new UnturnedPlayerBannedEvent(instigator, bannedPlayer, ip, reason, duration);

            Emit(@event);
        }

        private void Events_OnUnbanned(CSteamID unbannedPlayer)
        {
            var @event = new UnturnedPlayerUnbannedEvent(unbannedPlayer);

            Emit(@event);
        }

        private delegate void Banned(CSteamID instigator, CSteamID bannedPlayer, uint ip, string reason, uint duration);
        private static event Banned? OnBanned;

        private delegate void Unbanned(CSteamID unbannedPlayer);
        private static event Unbanned? OnUnbanned;

        [HarmonyPatch]
        [UsedImplicitly]
        internal static class Patches
        {
            [HarmonyCleanup]
            public static Exception? Cleanup(Exception ex, MethodBase original)
            {
                HarmonyExceptionHandler.ReportCleanupException(typeof(Patches), ex, original);
                return null;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(SteamBlacklist), nameof(SteamBlacklist.ban), typeof(CSteamID), typeof(uint), typeof(CSteamID), typeof(string), typeof(uint))]
            [HarmonyPostfix]
            public static void Ban(CSteamID playerID, uint ip, CSteamID judgeID, string reason, uint duration)
            {
                OnBanned?.Invoke(judgeID, playerID, ip, reason, duration);
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(SteamBlacklist), nameof(SteamBlacklist.unban))]
            [HarmonyPostfix]
            // ReSharper disable once InconsistentNaming
            public static void Unban(CSteamID playerID, bool __result)
            {
                if (__result)
                {
                    OnUnbanned?.Invoke(playerID);
                }
            }
        }
    }
}
