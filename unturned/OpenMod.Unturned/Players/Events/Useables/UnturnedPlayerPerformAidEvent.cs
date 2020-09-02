using OpenMod.API.Eventing;
using OpenMod.Unturned.Events;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Events.Useables
{
    public class UnturnedPlayerPerformAidEvent : UnturnedPlayerEvent, ICancellableEvent
    {
        public UnturnedPlayer Target { get; }

        public ItemConsumeableAsset ConsumeableAsset { get; }

        public bool IsCancelled { get; set; }

        public UnturnedPlayerPerformAidEvent(UnturnedPlayer player, UnturnedPlayer target, ItemConsumeableAsset consumeableAsset) : base(player)
        {
            Target = target;
            ConsumeableAsset = consumeableAsset;
        }
    }
}
