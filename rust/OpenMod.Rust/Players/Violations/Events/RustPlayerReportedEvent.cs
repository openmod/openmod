using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Violations.Events
{
    public class RustPlayerReportedEvent : RustPlayerEvent
    {
        public string TargetName { get; }
        public string TargetId { get; }
        public string Subject { get; }
        public string Message { get; }
        public string Type { get; }

        public RustPlayerReportedEvent(RustPlayer player, 
            string targetName, 
            string targetId, 
            string subject, 
            string message, 
            string type) : base(player)
        {
            TargetName = targetName;
            TargetId = targetId;
            Subject = subject;
            Message = message;
            Type = type;
        }
    }
}