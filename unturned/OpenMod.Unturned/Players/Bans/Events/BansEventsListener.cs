extern alias JetBrainsAnnotations;
using HarmonyLib;
using JetBrainsAnnotations::JetBrains.Annotations;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using Steamworks;
using System;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Nito.AsyncEx;
using OpenMod.API.Users;
using OpenMod.Core.Users;

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
            Provider.onBanPlayerRequested += OnBanPlayerRequested;
            Provider.onCheckBanStatusWithHWID += OnCheckBanStatus;
            Provider.onUnbanPlayerRequested += OnUnbanPlayerRequested;
            OnBanned += Events_OnBanned;
            OnUnbanned += Events_OnUnbanned;
        }

        public override void Unsubscribe()
        {
            Provider.onBanPlayerRequested -= OnBanPlayerRequested;
            Provider.onCheckBanStatusWithHWID -= OnCheckBanStatus;
            Provider.onUnbanPlayerRequested -= OnUnbanPlayerRequested;

            OnBanned -= Events_OnBanned;
            OnUnbanned -= Events_OnUnbanned;
        }

        private void OnBanPlayerRequested(CSteamID instigator, CSteamID playerToBan, uint ipToBan, ref string reason, ref uint duration, ref bool shouldVanillaBan) // lgtm [cs/too-many-ref-parameters]
        {
            var player = GetUnturnedPlayer(PlayerTool.getPlayer(playerToBan))!;
            var ipAddressToBan = IPAddress.Parse(ipToBan.ToString());

            var @event = new UnturnedPlayerBanningEvent(
                player,
                instigator.ToString(),
                KnownActorTypes.Player,
                Equals(ipAddressToBan, IPAddress.Any) ? null : ipAddressToBan,
                reason,
                TimeSpan.FromSeconds(duration))
            {
                IsCancelled = !shouldVanillaBan
            };

            Emit(@event);

            reason = @event.Reason ?? "";
            duration = (uint) @event.Duration.TotalSeconds;
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
#if !DEBUG
            [HarmonyCleanup]
            public static Exception? Cleanup(Exception ex)
            {
                return null;
            }
#endif

            [UsedImplicitly]
            [HarmonyPatch(typeof(SteamBlacklist), "ban", typeof(CSteamID), typeof(uint), typeof(CSteamID), typeof(string), typeof(uint))]
            [HarmonyPostfix]
            public static void Ban(CSteamID playerID, uint ip, CSteamID judgeID, string reason, uint duration)
            {
                OnBanned?.Invoke(judgeID, playerID, ip, reason, duration);
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(SteamBlacklist), "unban")]
            [HarmonyPostfix]
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
