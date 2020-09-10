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

                if (@event.CommandContext.Exception is CommandNotFoundException && R.Commands != null)
                {
                    IRocketPlayer rocketPlayer;
                    if (@event.Actor is UnturnedUser user)
                    {
                        var steamPlayer = user.Player.SteamPlayer;
                        if (!UnturnedPermissions.CheckPermissions(steamPlayer, $"/{@event.CommandContext.CommandAlias}"))
                        {
                            // command doesnt exist or no permission
                            return;
                        }

                        rocketPlayer = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                    }
                    else if (@event.Actor.Type.Equals(KnownActorTypes.Console, StringComparison.OrdinalIgnoreCase))
                    {
                        rocketPlayer = new ConsolePlayer();

                        var command = R.Commands.GetCommand(@event.CommandContext.CommandAlias.ToLower(CultureInfo.InvariantCulture));
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

                    if (string.IsNullOrEmpty(@event.CommandContext.CommandAlias))
                    {
                        Console.WriteLine("command alias is null or empty");
                        return;
                    }

                    var args = new List<string> { @event.CommandContext.CommandAlias };
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