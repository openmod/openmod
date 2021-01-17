using Cysharp.Threading.Tasks;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Unturned.Events;
using SDG.Unturned;
// ReSharper disable DelegateSubtraction

namespace OpenMod.Unturned.Players.Connections.Events
{
    internal class PlayerConnectionsEventsListener : UnturnedEventsListener
    {
        public PlayerConnectionsEventsListener(IOpenModHost openModHost,
            IEventBus eventBus,
            IUserManager userManager) : base(openModHost, eventBus, userManager)
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
                UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

                await UniTask.DelayFrame(1);

                UnturnedPlayerConnectedEvent @event = new UnturnedPlayerConnectedEvent(player);

                Emit(@event);
            }

            EmitPlayerConnected(steamPlayer).Forget();
        }

        private void OnPlayerDisconnected(SteamPlayer steamPlayer)
        {
            UnturnedPlayer player = GetUnturnedPlayer(steamPlayer);

            UnturnedPlayerDisconnectedEvent @event = new UnturnedPlayerDisconnectedEvent(player);

            Emit(@event);
        }
    }
}