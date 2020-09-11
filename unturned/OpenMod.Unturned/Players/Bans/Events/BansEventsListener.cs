using HarmonyLib;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Players.Bans.Events
{
    internal class BansEventsListener : UnturnedEventsListener
    {
        public BansEventsListener(IOpenModHost openModHost,
            IEventBus eventBus,
            IUserManager userManager) : base(openModHost, eventBus, userManager)
        {

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

        private void OnBanPlayerRequested(CSteamID instigator, CSteamID playerToBan, uint ipToBan, ref string reason, ref uint duration, ref bool shouldVanillaBan)
        {
            UnturnedPlayerBanningEvent @event = new UnturnedPlayerBanningEvent(instigator, playerToBan, ipToBan, reason, duration);

            Emit(@event);

            reason = @event.Reason;
            duration = @event.Duration;
            shouldVanillaBan = !@event.IsCancelled;
        }

        private void OnCheckBanStatus(SteamPlayerID playerId, uint remoteIP, ref bool isBanned, ref string banReason, ref uint banRemainingDuration)
        {
            UnturnedPlayerCheckingBanEvent @event = new UnturnedPlayerCheckingBanEvent(playerId, remoteIP, isBanned, banReason, banRemainingDuration);

            Emit(@event);

            isBanned = @event.IsBanned;
            banReason = @event.Reason;
            banRemainingDuration = @event.RemainingDuration;
        }

        private void OnUnbanPlayerRequested(CSteamID instigator, CSteamID playerToUnban, ref bool shouldVanillaUnban)
        {
            UnturnedPlayerUnbanningEvent @event = new UnturnedPlayerUnbanningEvent(instigator, playerToUnban);

            Emit(@event);

            shouldVanillaUnban = !@event.IsCancelled;
        }

        private void Events_OnBanned(CSteamID instigator, CSteamID bannedPlayer, uint ip, string reason, uint duration)
        {
            UnturnedPlayerBannedEvent @event = new UnturnedPlayerBannedEvent(instigator, bannedPlayer, ip, reason, duration);

            Emit(@event);
        }

        private void Events_OnUnbanned(CSteamID unbannedPlayer)
        {
            UnturnedPlayerUnbannedEvent @event = new UnturnedPlayerUnbannedEvent(unbannedPlayer);

            Emit(@event);
        }

        private delegate void Banned(CSteamID instigator, CSteamID bannedPlayer, uint ip, string reason, uint duration);
        private static event Banned OnBanned;

        private delegate void Unbanned(CSteamID unbannedPlayer);
        private static event Unbanned OnUnbanned;

        [HarmonyPatch]
        private class Patches
        {
            [HarmonyPatch(typeof(SteamBlacklist), "ban", typeof(CSteamID), typeof(uint), typeof(CSteamID), typeof(string), typeof(uint))]
            [HarmonyPostfix]
            private static void Ban(CSteamID playerID, uint ip, CSteamID judgeID, string reason, uint duration)
            {
                OnBanned?.Invoke(judgeID, playerID, ip, reason, duration);
            }

            [HarmonyPatch(typeof(SteamBlacklist), "unban")]
            [HarmonyPostfix]
            private static void Unban(CSteamID playerID, bool __result)
            {
                if (__result)
                {
                    OnUnbanned?.Invoke(playerID);
                }
            }
        }
    }
}
