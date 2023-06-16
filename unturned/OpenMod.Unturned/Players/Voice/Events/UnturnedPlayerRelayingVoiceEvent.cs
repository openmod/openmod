using OpenMod.API.Eventing;
using OpenMod.Unturned.Events;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Voice.Events
{
    public class UnturnedPlayerRelayingVoiceEvent : UnturnedPlayerEvent, ICancellableEvent
    {
        public bool WantsToUseWalkieTalkie { get; }
        public bool ShouldBroadcastOverRadio { get; set; }
        public PlayerVoice.RelayVoiceCullingHandler CullingHandler { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedPlayerRelayingVoiceEvent(UnturnedPlayer player, bool wantsToUseWalkieTalkie,
            bool shouldBroadcastOverRadio, PlayerVoice.RelayVoiceCullingHandler cullingHandler) : base(player)
        {
            WantsToUseWalkieTalkie = wantsToUseWalkieTalkie;
            ShouldBroadcastOverRadio = shouldBroadcastOverRadio;
            CullingHandler = cullingHandler;
        }
    }
}
