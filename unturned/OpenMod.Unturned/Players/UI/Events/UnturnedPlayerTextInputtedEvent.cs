namespace OpenMod.Unturned.Players.UI.Events
{
    public class UnturnedPlayerTextInputtedEvent : UnturnedPlayerUIEvent
    {
        public string TextInputName { get; }

        public string Text { get; }

        public UnturnedPlayerTextInputtedEvent(UnturnedPlayer player, string textInputName, string text) : base(player)
        {
            TextInputName = textInputName;
            Text = text;
        }
    }
}
