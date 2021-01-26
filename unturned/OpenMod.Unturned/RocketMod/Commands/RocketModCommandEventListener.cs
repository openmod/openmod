using System;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Eventing;
using OpenMod.Core.Commands;
using OpenMod.Core.Commands.Events;
using OpenMod.Core.Eventing;
using OpenMod.Core.Users;
using OpenMod.Unturned.Users;
using Rocket.API;
using Rocket.Core;
using Rocket.Unturned.Permissions;
using Rocket.Unturned.Player;

namespace OpenMod.Unturned.RocketMod.Commands
{
    [EventListenerLifetime(ServiceLifetime.Transient)]
    public class RocketModCommandEventListener : IEventListener<CommandExecutedEvent>
    {
        private const string s_RocketPrefix = "rocket:";
        private static readonly MethodInfo s_CheckPermissionsMethod;
        private static readonly Regex s_PrefixRegex;

        static RocketModCommandEventListener()
        {
            s_PrefixRegex = new Regex(Regex.Escape(s_RocketPrefix));
            s_CheckPermissionsMethod = typeof(UnturnedPermissions)
                .GetMethod("CheckPermissions", BindingFlags.Static | BindingFlags.NonPublic);
        }

        [EventListener(Priority = EventListenerPriority.Monitor)]
        public Task HandleEventAsync(object emitter, CommandExecutedEvent @event)
        {
            async UniTask Task()
            {
                // RocketMod commands must run on main thread
                await UniTask.SwitchToMainThread();

                if (@event.CommandContext.Exception is CommandNotFoundException && R.Commands != null)
                {
                    var commandAlias = @event.CommandContext.CommandAlias;
                    if (string.IsNullOrEmpty(commandAlias))
                    {
                        return;
                    }

                    var isRocketPrefixed = commandAlias.StartsWith(s_RocketPrefix);
                    if (isRocketPrefixed)
                    {
                        commandAlias = s_PrefixRegex.Replace(commandAlias, string.Empty, 1);
                    }

                    IRocketPlayer rocketPlayer;
                    if (@event.Actor is UnturnedUser user)
                    {
                        var steamPlayer = user.Player.SteamPlayer;
                        if (!(bool)s_CheckPermissionsMethod.Invoke(null, new object[] { steamPlayer, $"/{commandAlias}" }))
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
                        // unsupported user; do not handle
                        return;
                    }

                    var commandLine = @event.CommandContext.GetCommandLine();
                    if (isRocketPrefixed)
                    {
                        commandLine = s_PrefixRegex.Replace(commandLine, string.Empty, count: 1);
                    }

                    R.Commands.Execute(rocketPlayer, commandLine);
                    @event.ExceptionHandled = true;
                }
            }

            return Task().AsTask();
        }
    }
}