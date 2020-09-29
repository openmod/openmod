using OpenMod.API.Eventing;

namespace OpenMod.Rust.Players.Events
{
    public class RustPlayerVoiceReceivedEvent : RustPlayerEvent, ICancellableEvent
    {
        public byte[] VoiceData { get; }

        public bool IsCancelled { get; set; }
     
        public RustPlayerVoiceReceivedEvent(RustPlayer player, byte[] voiceData) : base(player)
        {
            VoiceData = voiceData;
        }
    }
}