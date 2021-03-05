extern alias JetBrainsAnnotations;
using JetBrainsAnnotations::JetBrains.Annotations;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using System;

namespace OpenMod.Unturned.Players.Crafting.Events
{
    [UsedImplicitly]
    internal class CraftingEventsListener : UnturnedEventsListener
    {
        public CraftingEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void Subscribe()
        {
            PlayerCrafting.onCraftBlueprintRequested += OnCraftBlueprintRequested;
        }

        public override void Unsubscribe()
        {
            PlayerCrafting.onCraftBlueprintRequested -= OnCraftBlueprintRequested;
        }

        private void OnCraftBlueprintRequested(PlayerCrafting crafting, ref ushort itemId, ref byte blueprintIndex, ref bool shouldAllow) // lgtm [cs/too-many-ref-parameters]
        {
            var player = GetUnturnedPlayer(crafting.player)!;
            var @event = new UnturnedPlayerCraftingEvent(player, itemId, blueprintIndex)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            itemId = @event.ItemId;
            blueprintIndex = @event.BlueprintIndex;
            shouldAllow = !@event.IsCancelled;
        }
    }
}
