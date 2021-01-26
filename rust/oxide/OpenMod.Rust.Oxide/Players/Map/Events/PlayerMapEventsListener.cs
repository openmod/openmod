using System.Collections.Generic;
using JetBrains.Annotations;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.Rust.Oxide.Events;
using OpenMod.Rust.Players;
using OpenMod.Rust.Players.Map.Events;
using Oxide.Core.Plugins;
using ProtoBuf;

namespace OpenMod.Rust.Oxide.Players.Map.Events
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal class PlayerMapEventsListener : OxideEventsListenerBase
    {
        public PlayerMapEventsListener(IEventBus eventBus, IOpenModHost openModHost)
            : base(eventBus, openModHost)
        {
        }

        [HookMethod("OnMapMarkerAdded")]
        private void OnMapMarkerAdded(BasePlayer player, MapNote note)
        {
            var @event = new RustPlayerMapMarkerAddedEvent(new RustPlayer(player), note);
            Emit(@event);
        }

        [HookMethod("OnMapMarkerAdd")]
        private object? OnMapMarkerAdd(BasePlayer player, MapNote note)
        {
            var @event = new RustPlayerMapMarkerAddingEvent(new RustPlayer(player), note);
            return EmitCancellableReturnsObject(@event);
        }

        [HookMethod("OnMapMarkerRemove")]
        private object? OnMapMarkerRemove(BasePlayer player, MapNote note)
        {
            var @event = new RustPlayerMapMarkerRemovingEvent(new RustPlayer(player), note);
            return EmitCancellableReturnsObject(@event);
        }

        [HookMethod("OnMapMarkersCleared")]
        private void OnMapMarkersCleared(BasePlayer player, List<MapNote> notes)
        {
            var @event = new RustPlayerMapMarkersClearedEvent(new RustPlayer(player), notes);
            Emit(@event);
        }

        [HookMethod("OnMapMarkersClear")]
        private object? OnMapMarkersClear(BasePlayer player, List<MapNote> notes)
        {
            var @event = new RustPlayerMapMarkersClearingEvent(new RustPlayer(player), notes);
            return EmitCancellableReturnsObject(@event);
        }
    }
}
