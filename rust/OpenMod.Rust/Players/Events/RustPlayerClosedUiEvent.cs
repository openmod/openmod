namespace OpenMod.Rust.Players.Events
{
    public class RustPlayerClosedUiEvent : RustPlayerEvent
    {
        public string Json { get; }

        public RustPlayerClosedUiEvent(RustPlayer player, string json) : base(player)
        {
            Json = json;
        }
    }
}