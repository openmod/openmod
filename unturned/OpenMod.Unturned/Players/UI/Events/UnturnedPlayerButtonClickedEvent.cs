namespace OpenMod.Unturned.Players.UI.Events
{
    public class UnturnedPlayerButtonClickedEvent : UnturnedPlayerUIEvent
    {
        public string ButtonName { get; }

        public UnturnedPlayerButtonClickedEvent(UnturnedPlayer player, string buttonName) : base(player)
        {
            ButtonName = buttonName;
        }
    }
}
