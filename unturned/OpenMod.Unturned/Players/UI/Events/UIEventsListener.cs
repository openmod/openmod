extern alias JetBrainsAnnotations;
using JetBrainsAnnotations::JetBrains.Annotations;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using System;

namespace OpenMod.Unturned.Players.UI.Events
{
    [UsedImplicitly]
    internal class UIEventsListener : UnturnedEventsListener
    {
        public UIEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        
        public override void Subscribe()
        {
            EffectManager.onEffectButtonClicked += OnEffectButtonClicked;
            EffectManager.onEffectTextCommitted += OnEffectTextCommitted;
        }

        public override void Unsubscribe()
        {
            EffectManager.onEffectButtonClicked -= OnEffectButtonClicked;
            EffectManager.onEffectTextCommitted -= OnEffectTextCommitted;
        }

        private void OnEffectButtonClicked(Player nativePlayer, string buttonName)
        {
            var player = GetUnturnedPlayer(nativePlayer);

            var @event = new UnturnedPlayerButtonClickedEvent(player!, buttonName);

            Emit(@event);
        }

        private void OnEffectTextCommitted(Player nativePlayer, string textInputName, string text)
        {
            var player = GetUnturnedPlayer(nativePlayer);

            var @event = new UnturnedPlayerTextInputtedEvent(player!, textInputName, text);

            Emit(@event);
        }
    }
}
