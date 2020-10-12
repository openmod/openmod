namespace OpenMod.Rust.Players.Events
{
    public class RustPlayerInputReceivedEvent : RustPlayerEvent
    {
        public InputState InputState { get; }

        public RustPlayerInputReceivedEvent(
            RustPlayer player,
            InputState inputState) : base(player)
        {
            InputState = inputState;
        }
    }
}