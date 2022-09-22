using System;
using System.Reflection;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using OpenMod.API.Eventing;
using OpenMod.Core.Commands;
using OpenMod.Core.Commands.Events;
using OpenMod.Core.Eventing;
using OpenMod.Core.Users;
using OpenMod.Unturned.Users;
using Rocket.API;
using Rocket.Core;
using Rocket.Core.Commands;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;

namespace OpenMod.Unturned.RocketMod.Commands
{
    public class RocketModExecuteCommandEventTrigger
    {
        public FieldInfo? RocketModExecuteEvent { get; }

        public RocketModExecuteCommandEventTrigger()
        {
            RocketModExecuteEvent = typeof(RocketCommandManager).GetField(nameof(RocketCommandManager.OnExecuteCommand), BindingFlags.Instance | BindingFlags.NonPublic);
        }

        [EventListener(Priority = EventListenerPriority.Monitor)]
        public Task HandleEventAsync(object? _, CommandExecutingEvent @event)
        {
            async UniTask Task()
            {
                //Just need to be handle if command exists
                if (@event.CommandContext.Exception is CommandNotFoundException)
                {
                    return;
                }

                var command = new RocketModCommandFromOpenMod(@event.CommandContext);

                await UniTask.SwitchToMainThread();
                if (R.Commands == null)
                {
                    return;
                }

                if (RocketModExecuteEvent?.GetValue(R.Commands) is not MulticastDelegate delegates)
                {
                    return;
                }

                IRocketPlayer rocketPlayer;
                if (@event.Actor is UnturnedUser user)
                {
                    rocketPlayer = UnturnedPlayer.FromSteamPlayer(user.Player.SteamPlayer);
                }
                else if (@event.Actor.Type.Equals(KnownActorTypes.Console, StringComparison.OrdinalIgnoreCase))
                {
                    rocketPlayer = new ConsolePlayer();
                }
                else if (@event.Actor.Type.Equals(KnownActorTypes.Rcon, StringComparison.OrdinalIgnoreCase))
                {
                    rocketPlayer = new RconPlayer(@event.Actor);
                }
                else
                {
                    return;
                }

                var cancel = @event.IsCancelled;
                var arguments = new object[]
                {
                    rocketPlayer,
                    command,
                    cancel
                };

                foreach (var targetDelegate in delegates.GetInvocationList())
                {
                    try
                    {
                        targetDelegate.Method.Invoke(targetDelegate.Target, arguments);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                    }
                }

                @event.IsCancelled = (bool)arguments[2];
            }

            return Task().AsTask();
        }
    }

}
