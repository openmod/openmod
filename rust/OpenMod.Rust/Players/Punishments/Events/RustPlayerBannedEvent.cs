using OpenMod.Core.Eventing;

namespace OpenMod.Rust.Players.Punishments.Events
{
    public class RustPlayerBannedEvent : Event
    {
        public string PlayerName { get; }
        public ulong Id { get; }
        public string Address { get; }
        public string Reason { get; }

        public RustPlayerBannedEvent(string playerName, ulong id, string address, string reason)
        {
            PlayerName = playerName;
            Id = id;
            Address = address;
            Reason = reason;
        }
    }
}