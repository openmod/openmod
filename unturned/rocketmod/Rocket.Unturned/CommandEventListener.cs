using System;
using System.Threading.Tasks;
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

namespace Rocket.Unturned
{
    // OPENMOD PATCH: Hook OpenMod command processing
    [EventListenerLifetime(ServiceLifetime.Transient)]
    public class CommandEventListener : IEventListener<CommandExecutedEvent>
    {
        [EventListener(Priority = Priority.Highest)]
        public async Task HandleEventAsync(object emitter, CommandExecutedEvent @event)
        {
            if (@event.CommandContext.Exception is CommandNotFoundException && R.Commands != null)
            {
                var text = @event.CommandContext.GetCommandLine();

                IRocketPlayer rocketPlayer = null;
                if (@event.Actor is UnturnedUser user)
                {
                    var player = user.SteamPlayer;
                    if (UnturnedPermissions.CheckPermissions(player, text))
                    {
                        rocketPlayer = UnturnedPlayer.FromSteamPlayer(player);
                    }
                }
                else if (@event.Actor.Type.Equals(KnownActorTypes.Console, StringComparison.OrdinalIgnoreCase))
                {
                    rocketPlayer = new ConsolePlayer();
                }

                if (rocketPlayer != null)
                {
                    R.Commands.Execute(rocketPlayer, text);
                    @event.CommandContext.Exception = null;
                }
            }
        }
    }
    // END OPENMOD PATCH: Hook OpenMod command processing
}