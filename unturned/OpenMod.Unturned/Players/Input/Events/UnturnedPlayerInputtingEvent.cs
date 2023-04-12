using OpenMod.Unturned.Events;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Input.Events
{
    public class UnturnedPlayerInputtingEvent : UnturnedPlayerEvent
    {
        public UnturnedPlayerInputtingEvent(UnturnedPlayer player, InputInfo? inputInfo, bool doOcclusionCheck, ERaycastInfoUsage usage) : base(player)
        {
            InputInfo = inputInfo;
            DoOcclusionCheck = doOcclusionCheck;
            Usage = usage;
        }

        /// <summary>
        /// Gets or sets the <see cref="InputInfo"/>.
        /// <remarks>Changes to the model is going to be applied after the event.</remarks>
        /// </summary>
        public InputInfo? InputInfo { get; set; }
        public bool DoOcclusionCheck { get; set; }
        public ERaycastInfoUsage Usage { get; }
    }
}