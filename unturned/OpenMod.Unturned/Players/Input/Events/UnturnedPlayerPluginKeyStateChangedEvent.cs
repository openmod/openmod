using OpenMod.Unturned.Events;

namespace OpenMod.Unturned.Players.Input.Events
{
    public class UnturnedPlayerPluginKeyStateChangedEvent : UnturnedPlayerEvent
    {
        public byte Key { get; }

        public bool State { get; }

        public UnturnedPlayerPluginKeyStateChangedEvent(UnturnedPlayer player, byte key, bool state) : base(player)
        {
            Key = key;
            State = state;
        }
    }
}