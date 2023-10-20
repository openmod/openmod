using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Eventing;
using OpenMod.API.Localization;
using OpenMod.Core.Commands;
using OpenMod.Core.Commands.Events;
using OpenMod.Core.Cooldowns;
using OpenMod.Core.Eventing;
using OpenMod.Core.Users;
using OpenMod.Unturned.RocketMod.Permissions;
using OpenMod.Unturned.Users;
using Rocket.API;
using Rocket.Core;
using Rocket.Unturned.Permissions;
using Rocket.Unturned.Player;

namespace OpenMod.Unturned.RocketMod.Commands
{
    public class RocketModCommandEventListener
    {
        private const string c_RocketPrefix = "rocket:";
        private readonly ICommandCooldownStore m_CommandCooldownStore;
        private readonly IOpenModStringLocalizer m_StringLocalizer;
        private static readonly MethodInfo s_CheckPermissionsMethod;
        private static readonly Regex s_PrefixRegex;

        static RocketModCommandEventListener()
        {
            s_PrefixRegex = new Regex(Regex.Escape(c_RocketPrefix));
            s_CheckPermissionsMethod = typeof(UnturnedPermissions)
                .GetMethod("CheckPermissions", BindingFlags.Static | BindingFlags.NonPublic)!;
        }

        public RocketModCommandEventListener(
            ICommandCooldownStore commandCooldownStore,
            IOpenModStringLocalizer stringLocalizer)
        {
            m_CommandCooldownStore = commandCooldownStore;
            m_StringLocalizer = stringLocalizer;
        }

        [EventListener(Priority = EventListenerPriority.Monitor)]
        public Task HandleEventAsync(object? _, CommandExecutedEvent @event)
        {
            async UniTask Task()
            {
                if (@event.CommandContext.Exception is not CommandNotFoundException)
                {
                    return;
                }

                // RocketMod commands must run on main thread
                await UniTask.SwitchToMainThread();

                if (R.Commands == null)
                {
                    return;
                }


                if (!await CheckCooldownAsync(@event))
                {
                    return;
                }

                var commandAlias = @event.CommandContext.CommandAlias;
                if (string.IsNullOrEmpty(commandAlias))
                {
                    return;
                }

                var isRocketPrefixed = commandAlias.StartsWith(c_RocketPrefix);
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
                }
                else if (@event.Actor.Type.Equals(KnownActorTypes.Rcon, StringComparison.OrdinalIgnoreCase))
                {
                    rocketPlayer = new RconPlayer(@event.Actor);

                    if (!CheckRconPermission(rocketPlayer, commandAlias))
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

            return Task().AsTask();
        }

        private const string c_RocketCooldownsFormat = "Rocket.{0}";
        private async Task<bool> CheckCooldownAsync(CommandExecutedEvent @event)
        {
            const string rocketPrefix = "rocket:";

            var commandContext = @event.CommandContext;
            var commandAlias = commandContext.CommandAlias;
            if (string.IsNullOrEmpty(commandAlias))
            {
                return true;
            }

            if (@event.Actor is not UnturnedUser user)
            {
                return true;
            }

            if (commandAlias.StartsWith(rocketPrefix))
            {
                commandAlias = commandAlias.Replace(rocketPrefix, string.Empty);
            }

            var steamPlayer = user.Player.SteamPlayer;
            var rocketPlayer = UnturnedPlayer.FromSteamPlayer(steamPlayer);
            var command = R.Commands.GetCommand(commandAlias.ToLower());
            if (command == null || !R.Permissions.HasPermission(rocketPlayer, command))
            {
                return true;
            }

            var commandId = string.Format(c_RocketCooldownsFormat, command.Name);
            var cooldownSpan = await m_CommandCooldownStore.GetCooldownSpanAsync(commandContext.Actor, commandId);

            if (cooldownSpan.HasValue)
            {
                var lastExecuted = await m_CommandCooldownStore.GetLastExecutedAsync(@event.Actor, commandId);
                if (lastExecuted.HasValue)
                {
                    var spanSinceLast = DateTime.Now - lastExecuted.Value;

                    if (spanSinceLast < cooldownSpan)
                    {
                        @event.CommandContext.Exception = new UserFriendlyException(
                            m_StringLocalizer["commands:errors:cooldown",
                                new { TimeLeft = cooldownSpan - spanSinceLast }]!);

                        @event.ExceptionHandled = false;
                        return false;
                    }
                }

                await m_CommandCooldownStore.RecordExecutionAsync(commandContext.Actor, commandId, DateTime.Now);
            }

            return true;
        }

        private bool CheckRconPermission(IRocketPlayer player, string commandAlias)
        {
            if (R.Permissions.GetType() != typeof(RocketPermissionProxyProvider))
            {
                return true;
            }

            var command = R.Commands.GetCommand(commandAlias);
            return command != null && R.Permissions.HasPermission(player, command);
        }
    }
}