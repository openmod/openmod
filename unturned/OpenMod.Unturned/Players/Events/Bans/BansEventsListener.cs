using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Players.Events.Bans
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
        }

        public override void Unsubscribe()
        {
            Provider.onBanPlayerRequested -= OnBanPlayerRequested;
            Provider.onCheckBanStatusWithHWID -= OnCheckBanStatus;
            Provider.onUnbanPlayerRequested -= OnUnbanPlayerRequested;
        }

        private void OnBanPlayerRequested(CSteamID instigator, CSteamID playerToBan, uint ipToBan, ref string reason, ref uint duration, ref bool shouldVanillaBan)
        {
            UnturnedPlayerBanningEvent @event = new UnturnedPlayerBanningEvent(instigator, playerToBan, ipToBan, reason, duration, shouldVanillaBan);

            Emit(@event);

            reason = @event.Reason;
            duration = @event.Duration;
            shouldVanillaBan = @event.ShouldVanillaBan;
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
            UnturnedPlayerUnbanningEvent @event = new UnturnedPlayerUnbanningEvent(instigator, playerToUnban, shouldVanillaUnban);

            Emit(@event);

            shouldVanillaUnban = @event.ShouldVanillaUnban;
        }
    }
}
