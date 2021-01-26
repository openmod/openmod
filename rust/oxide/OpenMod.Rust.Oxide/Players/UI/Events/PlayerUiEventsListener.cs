using JetBrains.Annotations;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.Rust.Oxide.Events;
using OpenMod.Rust.Players;
using OpenMod.Rust.Players.UI.Events;
using Oxide.Core.Plugins;

namespace OpenMod.Rust.Oxide.Players.UI.Events
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal class PlayerUiEventsListener : OxideEventsListenerBase
    {
        public PlayerUiEventsListener(IEventBus eventBus, IOpenModHost openModHost) : base(eventBus, openModHost)
        {
        }

        [HookMethod("OnDestroyUI")]
        private void OnDestroyUI(BasePlayer player, string json)
        {
            var @event = new RustPlayerClosedUiEvent(new RustPlayer(player), json);
            Emit(@event);
        }

        [HookMethod("CanUseUI")]
        private object? CanUseUI(BasePlayer player, string json)
        {
            var @event = new RustPlayerOpeningUiEvent(new RustPlayer(player), json);
            return EmitCancellableReturnsObject(@event);
        }
    }
}
