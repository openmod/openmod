extern alias JetBrainsAnnotations;
using Cysharp.Threading.Tasks;
using JetBrainsAnnotations::JetBrains.Annotations;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using System;

namespace OpenMod.Unturned.Players.Connections.Events
{
    [UsedImplicitly]
    internal class PlayerConnectionsEventsListener : UnturnedEventsListener
    {
        public PlayerConnectionsEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void Subscribe()
        {
            Provider.onEnemyConnected += OnPlayerConnected;
            Provider.onEnemyDisconnected += OnPlayerDisconnected;
        }

        public override void Unsubscribe()
        {
            Provider.onEnemyConnected -= OnPlayerConnected;
            Provider.onEnemyDisconnected -= OnPlayerDisconnected;
        }

        private void OnPlayerConnected(SteamPlayer steamPlayer)
        {
            async UniTaskVoid EmitPlayerConnected(SteamPlayer nativePlayer)
            {
                var player = GetUnturnedPlayer(nativePlayer)!;

                await UniTask.DelayFrame(1);

                var @event = new UnturnedPlayerConnectedEvent(player);
                Emit(@event);
            }

            EmitPlayerConnected(steamPlayer).Forget();
        }

        private void OnPlayerDisconnected(SteamPlayer steamPlayer)
        {
            var player = GetUnturnedPlayer(steamPlayer)!;

            var @event = new UnturnedPlayerDisconnectedEvent(player);
            Emit(@event);
        }
    }
}