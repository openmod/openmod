namespace OpenMod.Rust.Players.Events
{
    public class RustPlayerInfoSetEvent : RustPlayerEvent
    {
        public string Key { get; }
        public string Value { get; }

        public RustPlayerInfoSetEvent(
            RustPlayer player,
            string key,
            string @value) : base(player)
        {
            Key = key;
            Value = value;
        }
    }
}