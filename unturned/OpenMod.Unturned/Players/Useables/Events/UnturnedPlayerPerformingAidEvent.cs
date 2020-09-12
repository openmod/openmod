using OpenMod.API.Eventing;
using OpenMod.Unturned.Events;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Useables.Events
{
    public class UnturnedPlayerPerformingAidEvent : UnturnedPlayerEvent, ICancellableEvent
    {
        public UnturnedPlayer Target { get; }

        public ItemConsumeableAsset ConsumeableAsset { get; }

        public bool IsCancelled { get; set; }

        public UnturnedPlayerPerformingAidEvent(UnturnedPlayer player, UnturnedPlayer target, ItemConsumeableAsset consumeableAsset) : base(player)
        {
            Target = target;
            ConsumeableAsset = consumeableAsset;
        }
    }
}
