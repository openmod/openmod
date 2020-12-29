using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Communications.Events
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