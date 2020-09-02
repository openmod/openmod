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
            UnturnedPlayerBanEvent @event = new UnturnedPlayerBanEvent(instigator, playerToBan, ipToBan, reason, duration, shouldVanillaBan);

            Emit(@event);

            reason = @event.Reason;
            duration = @event.Duration;
            shouldVanillaBan = @event.ShouldVanillaBan;
        }

        private void OnCheckBanStatus(SteamPlayerID playerId, uint remoteIP, ref bool isBanned, ref string banReason, ref uint banRemainingDuration)
        {
            UnturnedPlayerCheckBanEvent @event = new UnturnedPlayerCheckBanEvent(playerId, remoteIP, isBanned, banReason, banRemainingDuration);

            Emit(@event);

            isBanned = @event.IsBanned;
            banReason = @event.Reason;
            banRemainingDuration = @event.RemainingDuration;
        }

        private void OnUnbanPlayerRequested(CSteamID instigator, CSteamID playerToUnban, ref bool shouldVanillaUnban)
        {
            UnturnedPlayerUnbanEvent @event = new UnturnedPlayerUnbanEvent(instigator, playerToUnban, shouldVanillaUnban);

            Emit(@event);

            shouldVanillaUnban = @event.ShouldVanillaUnban;
        }
    }
}
