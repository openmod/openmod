using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Eventing;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using OpenMod.Core.Commands.Events;
using OpenMod.Core.Eventing;
using OpenMod.Core.Users;
using OpenMod.Unturned.Users;
using Rocket.API;
using Rocket.Core;
using Rocket.Unturned.Permissions;
using Rocket.Unturned.Player;

namespace Rocket.Unturned.Commands
{
    // OPENMOD PATCH: Hook OpenMod command processing hook
    [EventListenerLifetime(ServiceLifetime.Transient)]
    public class CommandEventListener : IEventListener<CommandExecutedEvent>
    {
        [EventListener(Priority = EventListenerPriority.Highest)]
        public Task HandleEventAsync(object emitter, CommandExecutedEvent @event)
        {
            async UniTask Task()
            {
                // RocketMod commands must run on main thread
                await UniTask.SwitchToMainThread();
                const string rocketPrefix = "rocket:";

                if (@event.CommandContext.Exception is CommandNotFoundException && R.Commands != null)
                {
                    var commandAlias = @event.CommandContext.CommandAlias;
                    if (string.IsNullOrEmpty(commandAlias))
                    {
                        return;
                    }

                    if (commandAlias.StartsWith(rocketPrefix))
                    {
                        commandAlias = commandAlias.Replace(rocketPrefix, string.Empty);
                    }

                    IRocketPlayer rocketPlayer;
                    if (@event.Actor is UnturnedUser user)
                    {
                        var steamPlayer = user.Player.SteamPlayer;
                        if (!UnturnedPermissions.CheckPermissions(steamPlayer, $"/{commandAlias}"))
                        {
                            // command doesnt exist or no permission
                            @event.ExceptionHandled = true;
                            return;
                        }

                        rocketPlayer = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                    }
                    else if (@event.Actor.Type.Equals(KnownActorTypes.Console, StringComparison.OrdinalIgnoreCase))
                    {
                        rocketPlayer = new ConsolePlayer();

                        var command = R.Commands.GetCommand(commandAlias.ToLower(CultureInfo.InvariantCulture));
                        if (command == null)
                        {
                            return;
                        }
                    }
                    else
                    {
                        // unsupported user
                        return;
                    }

                    var args = new List<string> { commandAlias };
                    args.AddRange(@event.CommandContext.Parameters);

                    R.Commands.Execute(rocketPlayer, args.ToArray());
                    @event.ExceptionHandled = true;
                }
            }

            return Task().AsTask();
        }
    }
    // END OPENMOD PATCH: Hook OpenMod command processing hook
}