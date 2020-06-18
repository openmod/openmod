using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;
using Rocket.Core.Extensions;
using Rocket.API;
using Rocket.API.Extensions;
using Rocket.Core.Logging;

namespace Rocket.Unturned.Events
{
    public sealed class UnturnedEvents : MonoBehaviour, IRocketImplementationEvents
    {
        private static UnturnedEvents Instance;
        private void Awake()
        {
            Instance = this;

            // OPENMOD PATCH: Extraced callbacks to methods 
            Provider.onServerDisconnected += OnServerDisconnected;
            Provider.onServerShutdown += OnServerShutdown;
            Provider.onServerConnected += OnServerConnected;
            DamageTool.playerDamaged += OnUnturnedPlayerDamaged;
            // END OPENMOD PATCH: Extraced callbacks to methods 
        }

        // OPENMOD PATCH: Unregister callbacks on destroy
        private void OnDestroy()
        {
            Instance = null;

            Provider.onServerDisconnected -= OnServerDisconnected;
            Provider.onServerShutdown -= OnServerShutdown;
            Provider.onServerConnected -= OnServerConnected;
            DamageTool.playerDamaged -= OnUnturnedPlayerDamaged;
        }
        // END OPENMOD PATCH: Extraced callbacks to methods 

        private void OnUnturnedPlayerDamaged(SDG.Unturned.Player player, ref EDeathCause cause, ref ELimb limb, ref CSteamID killer, ref Vector3 direction, ref float damage, ref float times, ref bool canDamage)
        {
            if (OnPlayerDamaged != null)
            {
                if (player != null && killer != CSteamID.Nil && killer != null)
                {
                    UnturnedPlayer getterDamage = UnturnedPlayer.FromPlayer(player);
                    UnturnedPlayer senderDamage = UnturnedPlayer.FromCSteamID(killer);
                    OnPlayerDamaged.TryInvoke(getterDamage, cause, limb, senderDamage, direction, damage, times, canDamage);
                }
            }
        }

        private void OnServerConnected(CSteamID steamid)
        {
            if (steamid != CSteamID.Nil)
            {
                UnturnedPlayer p = (UnturnedPlayer)UnturnedPlayer.FromCSteamID(steamid);
                p.Player.gameObject.TryAddComponent<UnturnedPlayerFeatures>();
                p.Player.gameObject.TryAddComponent<UnturnedPlayerMovement>();
                p.Player.gameObject.TryAddComponent<UnturnedPlayerEvents>();
                OnBeforePlayerConnected.TryInvoke(p);
            }
        }

        private void OnServerShutdown()
        {
            onShutdown.TryInvoke();
        }

        private void OnServerDisconnected(CSteamID steamid)
        {
            if (steamid != CSteamID.Nil)
            {
                OnPlayerDisconnected.TryInvoke(UnturnedPlayer.FromCSteamID(steamid));
            }
        }

        public delegate void PlayerDisconnected(UnturnedPlayer player);
        public event PlayerDisconnected OnPlayerDisconnected;

        public delegate void OnPlayerGetDamage(UnturnedPlayer player, ref EDeathCause cause, ref ELimb limb, ref UnturnedPlayer killer, ref Vector3 direction, ref float damage, ref float times, ref bool canDamage);
        public static event OnPlayerGetDamage OnPlayerDamaged;

        private event ImplementationShutdown onShutdown;
        public event ImplementationShutdown OnShutdown
        {
            add
            {
                onShutdown += value;
            }

            remove
            {
                onShutdown -= value;
            }
        }

        internal static void triggerOnPlayerConnected(UnturnedPlayer player)
        {
            Instance.OnPlayerConnected.TryInvoke(player);
        }

        public delegate void PlayerConnected(UnturnedPlayer player);
        public event PlayerConnected OnPlayerConnected;
        public event PlayerConnected OnBeforePlayerConnected;
    }
}