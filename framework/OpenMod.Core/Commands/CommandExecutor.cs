using System;
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
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Transient, Priority = Priority.Lowest)]
    public class CommandExecutor : ICommandExecutor
    {
        private readonly IRuntime m_Runtime;
        private readonly ILifetimeScope m_LifetimeScope;
        private readonly ICommandStore m_CommandStore;
        private readonly ICommandPermissionBuilder m_CommandPermissionBuilder;
        private readonly IEventBus m_EventBus;

        public CommandExecutor(
            IRuntime runtime,
            ILifetimeScope lifetimeScope,
            ICommandStore commandStore,
            ICommandPermissionBuilder commandPermissionBuilder,
            IEventBus eventBus)
        {
            m_Runtime = runtime;
            m_LifetimeScope = lifetimeScope;
            m_CommandStore = commandStore;
            m_CommandPermissionBuilder = commandPermissionBuilder;
            m_EventBus = eventBus;
        }

        public async Task<ICommandContext> ExecuteAsync(ICommandActor actor, string[] args, string prefix)
        {
            if (args == null || args.Length == 0)
            {
                throw new Exception("Can not execute command with null or empty args");
            }

            var logger = m_LifetimeScope.Resolve<ILogger<CommandExecutor>>();
            logger.LogInformation($"Actor {actor.Type}/{actor.DisplayName} ({actor.Id}) has executed command \"{string.Join(" ", args)}\".");
            
            var currentCommandAccessor = m_LifetimeScope.Resolve<ICurrentCommandContextAccessor>();
            var commandsRegistrations = m_CommandStore.Commands;
            var commandContextBuilder = m_LifetimeScope.Resolve<ICommandContextBuilder>();
            var permissionChecker = m_LifetimeScope.Resolve<IPermissionChecker>();
            var stringLocalizer = m_LifetimeScope.Resolve<IOpenModStringLocalizer>();
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

                var permission = m_CommandPermissionBuilder.GetPermission(commandContext.CommandRegistration);
                if (!string.IsNullOrWhiteSpace(permission) && await permissionChecker.CheckPermissionAsync(actor, permission) != PermissionGrantResult.Grant)
                {
                    throw new NotEnoughPermissionException(permission, stringLocalizer);
                }

                var command = commandContext.CommandRegistration.Instantiate(commandContext.ServiceProvider);
                await command.ExecuteAsync();

                currentCommandAccessor.Context = null;
            }
            catch (UserFriendlyException ex)
            {
                await actor.PrintMessageAsync(ex.Message, Color.DarkRed);
                commandContext.Exception = ex;
            }
            catch (Exception ex)
            {
                await actor.PrintMessageAsync("An internal error occured during the command execution.", Color.DarkRed);
                logger.LogError(ex, $"Exception occured on command \"{string.Join(" ", args)}\" by actor {actor.Type}/{actor.DisplayName} ({actor.Id})");
                commandContext.Exception = ex;

#if DEBUG
                throw; // in debug mode we want to debug such exceptions instead of catching them
#endif
            }
            finally
            {
                var commandExecutedEvent = new CommandExecutedEvent(actor, commandContext);
                await m_EventBus.EmitAsync(m_Runtime, this, commandExecutedEvent);

                await commandContext.DisposeAsync();
            }

            return commandContext;
        }
    }
}