using JetBrains.Annotations;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using System;

namespace OpenMod.Unturned.Players.Voice.Events
{
    [UsedImplicitly]
    internal class PlayerVoiceEventsListener : UnturnedPlayerEventsListener
    {
        public PlayerVoiceEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            SubscribePlayer<Talked>(
                static (player, handler) => player.voice.onTalkingChanged += handler,
                static (player, handler) => player.voice.onTalkingChanged -= handler,
                player => isTalking => OnTalkingChanged(player, isTalking)
            );
        }

        public override void Subscribe()
        {
            PlayerVoice.onRelayVoice += PlayerVoice_onRelayVoice;
        }

        private void PlayerVoice_onRelayVoice(PlayerVoice speaker, bool wantsToUseWalkieTalkie, ref bool shouldAllow, ref bool shouldBroadcastOverRadio, ref PlayerVoice.RelayVoiceCullingHandler cullingHandler)
        {
            var player = GetUnturnedPlayer(speaker.player)!;

            var @event = new UnturnedPlayerRelayingVoiceEvent(player, wantsToUseWalkieTalkie, shouldBroadcastOverRadio, cullingHandler)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            shouldBroadcastOverRadio = @event.ShouldBroadcastOverRadio;
            shouldAllow = !@event.IsCancelled;
            cullingHandler = @event.CullingHandler;
        }

        public override void Unsubscribe()
        {
            PlayerVoice.onRelayVoice -= PlayerVoice_onRelayVoice;
        }

        private void OnTalkingChanged(Player nativePlayer, bool isTalking)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;

            var @event = new UnturnedPlayerTalkingUpdatedEvent(player, isTalking);

            Emit(@event);
        }
    }
}
