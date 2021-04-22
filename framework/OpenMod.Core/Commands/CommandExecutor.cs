using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using Autofac;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Eventing;
using OpenMod.API.Ioc;
using OpenMod.API.Localization;
using OpenMod.API.Permissions;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands.Events;

namespace OpenMod.Core.Commands
{
    [OpenModInternal]
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Transient, Priority = Priority.Lowest)]
    public class CommandExecutor : ICommandExecutor
    {
        private readonly IRuntime m_Runtime;
        private readonly ILifetimeScope m_LifetimeScope;
        private readonly ICommandStore m_CommandStore;
        private readonly ICommandPermissionBuilder m_CommandPermissionBuilder;
        private readonly IEventBus m_EventBus;
        private readonly ILogger<CommandExecutor> m_Logger;

        public CommandExecutor(
            IRuntime runtime,
            ILifetimeScope lifetimeScope,
            ICommandStore commandStore,
            ICommandPermissionBuilder commandPermissionBuilder,
            IEventBus eventBus,
            ILogger<CommandExecutor> logger)
        {
            m_Runtime = runtime;
            m_LifetimeScope = lifetimeScope;
            m_CommandStore = commandStore;
            m_CommandPermissionBuilder = commandPermissionBuilder;
            m_EventBus = eventBus;
            m_Logger = logger;
        }

        public async Task<ICommandContext> ExecuteAsync(ICommandActor actor, string[] args, string prefix)
        {
            if (args == null || args.Length == 0)
            {
                throw new Exception("Cannot execute command with null or empty args.");
            }

            m_Logger.LogInformation("Actor {ActorType}/{ActorName} has executed command \"{Command}\"",
                actor.Type, actor.FullActorName, string.Join(" ", args));

            var currentCommandAccessor = m_LifetimeScope.Resolve<ICurrentCommandContextAccessor>();
            var commandContextBuilder = m_LifetimeScope.Resolve<ICommandContextBuilder>();
            var stringLocalizer = m_LifetimeScope.Resolve<IOpenModStringLocalizer>();

            var commandsRegistrations = await m_CommandStore.GetCommandsAsync();
            var commandContext = commandContextBuilder.CreateContext(actor, args, prefix, commandsRegistrations);
            var commandExecutingEvent = new CommandExecutingEvent(actor, commandContext);
            await m_EventBus.EmitAsync(m_Runtime, this, commandExecutingEvent);

            if (commandExecutingEvent.IsCancelled)
            {
                return commandExecutingEvent.CommandContext;
            }

            try
            {
                if (commandContext.Exception != null)
                {
                    throw commandContext.Exception;
                }

                currentCommandAccessor.Context = commandContext;

                var permission = m_CommandPermissionBuilder.GetPermission(commandContext.CommandRegistration!);
                var permissionChecker = m_Runtime.LifetimeScope.Resolve<IPermissionChecker>();

                if (!string.IsNullOrWhiteSpace(permission) && await permissionChecker.CheckPermissionAsync(actor, permission) != PermissionGrantResult.Grant)
                {
                    throw new NotEnoughPermissionException(stringLocalizer, permission);
                }

                var sw = new Stopwatch();
                sw.Start();
                var command = commandContext.CommandRegistration!.Instantiate(commandContext.ServiceProvider);
                await command.ExecuteAsync();
                m_Logger.LogDebug("Command \"{Command}\" executed in {Ms}ms",
                    string.Join(" ", args), sw.ElapsedMilliseconds);

                currentCommandAccessor.Context = null;
            }
            catch (UserFriendlyException ex)
            {
                commandContext.Exception = ex;
            }
            catch (Exception ex)
            {
                commandContext.Exception = ex;
            }
            finally
            {
                var commandExecutedEvent = new CommandExecutedEvent(actor, commandContext);
                await m_EventBus.EmitAsync(m_Runtime, this, commandExecutedEvent);

                if (commandContext.Exception != null && !commandExecutedEvent.ExceptionHandled)
                {
                    if (commandContext.Exception is UserFriendlyException)
                    {
                        await actor.PrintMessageAsync(commandContext.Exception.Message, Color.DarkRed);
                    }
                    else
                    {
                        await actor.PrintMessageAsync("An internal error occured during the command execution.", Color.DarkRed);
                        m_Logger.LogError(commandContext.Exception,
                            "Exception occured on command \"{Command}\" by actor {ActorType}/{ActorName} ({ActorId})",
                            string.Join(" ", args), actor.Type, actor.DisplayName, actor.Id);
                    }
                }

                await commandContext.DisposeAsync();
            }

            return commandContext;
        }
    }
}