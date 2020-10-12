using ConVar;
using OpenMod.API.Eventing;

namespace OpenMod.Rust.Players.Events
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