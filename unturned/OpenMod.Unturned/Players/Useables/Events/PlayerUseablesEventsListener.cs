extern alias JetBrainsAnnotations;
using JetBrainsAnnotations::JetBrains.Annotations;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using System;

namespace OpenMod.Unturned.Players.Useables.Events
{
    [UsedImplicitly]
    internal class PlayerUseablesEventsListener : UnturnedEventsListener
    {
        public PlayerUseablesEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void Subscribe()
        {
            UseableConsumeable.onPerformingAid += OnPerformingAid;
        }

        public override void Unsubscribe()
        {
            UseableConsumeable.onPerformingAid -= OnPerformingAid;
        }

        private void OnPerformingAid(Player nativeInstigator, Player nativeTarget, ItemConsumeableAsset asset, ref bool shouldAllow)
        {
            var instigator = GetUnturnedPlayer(nativeInstigator);
            var target = GetUnturnedPlayer(nativeTarget);

            var @event = new UnturnedPlayerPerformingAidEvent(instigator!, target!, asset)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }
    }
}
