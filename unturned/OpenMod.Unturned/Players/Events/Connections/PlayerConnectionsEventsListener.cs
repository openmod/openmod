using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Unturned.Events;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Events.Connections
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
            UnturnedPlayer player = GetUnturnedPlayer(steamPlayer);

            UnturnedPlayerConnectedEvent @event = new UnturnedPlayerConnectedEvent(player);

            Emit(@event);
        }

        private void OnPlayerDisconnected(SteamPlayer steamPlayer)
        {
            UnturnedPlayer player = GetUnturnedPlayer(steamPlayer);

            UnturnedPlayerDisconnectedEvent @event = new UnturnedPlayerDisconnectedEvent(player);

            Emit(@event);
        }
    }
}