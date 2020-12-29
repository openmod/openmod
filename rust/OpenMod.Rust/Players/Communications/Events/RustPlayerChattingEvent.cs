using ConVar;
using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Communications.Events
{
    public class RustPlayerChattingEvent : RustPlayerEvent, ICancellableEvent
    {
        public string Message { get; }
        public Chat.ChatChannel Channel { get; }
        public bool IsCancelled { get; set; }

        public RustPlayerChattingEvent(RustPlayer player, 
            string message, 
            Chat.ChatChannel channel) : base(player)
        {
            Message = message;
            Channel = channel;
        }
    }
}