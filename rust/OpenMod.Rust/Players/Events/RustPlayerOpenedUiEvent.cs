namespace OpenMod.Rust.Players.Events
{
    public class RustPlayerOpenedUiEvent : RustPlayerEvent
    {
        public string Json { get; }

        public RustPlayerOpenedUiEvent(RustPlayer player, string json) : base(player)
        {
            Json = json;
        }
    }
}