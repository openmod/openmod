using Rocket.API;
using Rocket.API.Extensions;
using Rocket.Core.Logging;
using Rocket.Core.Utils;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Rocket.Unturned.Plugins
{
    public sealed class PluginUnturnedPlayerComponentManager : MonoBehaviour
    {
        private Assembly assembly;
        private List<Type> unturnedPlayerComponents = new List<Type>();
        
        private void OnDisable()
        {
            try
            {
                U.Events.OnPlayerConnected -= addPlayerComponents;
                unturnedPlayerComponents = unturnedPlayerComponents.Where(p => p.Assembly != assembly).ToList();
                List<Type> playerComponents = RocketHelper.GetTypesFromParentClass(assembly, typeof(UnturnedPlayerComponent));
                foreach (Type playerComponent in playerComponents)
                {
                    // OPENMOD PATCH: Remove Player components on disable
                    Provider.clients.ForEach(p => p.player.gameObject.TryRemoveComponent(playerComponent));
                    // END OPENMOD PATCH: Remove Player components on disable
                }
            }
            catch (Exception ex)
            {
                Core.Logging.Logger.LogException(ex);
            }
        }

        private void OnEnable()
        {
            try
            {  
                IRocketPlugin plugin = GetComponent<IRocketPlugin>();
                assembly = plugin.GetType().Assembly;

                U.Events.OnBeforePlayerConnected += addPlayerComponents;
                unturnedPlayerComponents.AddRange(RocketHelper.GetTypesFromParentClass(assembly, typeof(UnturnedPlayerComponent)));

                foreach (Type playerComponent in unturnedPlayerComponents)
                {
                    Core.Logging.Logger.Log("Adding UnturnedPlayerComponent: "+playerComponent.Name);

                    // OPENMOD PATCH: Add player components on enable
                    Provider.clients.ForEach(p => p.player.gameObject.TryAddComponent(playerComponent));
                    // END OPENMOD PATCH: Add player components on enable

                }
            }
            catch (Exception ex)
            {
                Core.Logging.Logger.LogException(ex);
            }
        }

        private void addPlayerComponents(IRocketPlayer p)
        {
            foreach (Type component in unturnedPlayerComponents)
            {
                ((UnturnedPlayer)p).Player.gameObject.AddComponent(component);
            }
        }
    }
}