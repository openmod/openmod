using OpenMod.API.Eventing;
using OpenMod.Unturned.Events;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Voice.Events
{
    public class UnturnedPlayerVoiceEvent : UnturnedPlayerEvent, ICancellableEvent
    {
        public bool WantsToUseWalkieTalkie { get; }
        public bool ShouldBroadcastOverRadio { get; set; }
        public bool IsCancelled { get; set; }

        public UnturnedPlayerVoiceEvent(UnturnedPlayer player, bool wantsToUseWalkieTalkie, bool shouldBroadcastOverRadio) : base(player)
        {
            WantsToUseWalkieTalkie = wantsToUseWalkieTalkie;
            ShouldBroadcastOverRadio = shouldBroadcastOverRadio;
        }
    }
}
