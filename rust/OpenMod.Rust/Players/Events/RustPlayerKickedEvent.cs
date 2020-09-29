namespace OpenMod.Rust.Players.Events
{
    public class RustPlayerKickedEvent : RustPlayerEvent
    {
        public string Reason { get; }

        public RustPlayerKickedEvent(
            RustPlayer player, 
            string reason) : base(player)
        {
            Reason = reason;
        }
    }
}