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
                throw new Exception("Can not execute command with null or empty args");
            }

            var logger = m_LifetimeScope.Resolve<ILogger<CommandExecutor>>();
            logger.LogInformation($"Actor {actor.Type}/{actor.DisplayName} ({actor.Id}) has executed command \"{string.Join(" ", args)}\".");

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

                var permission = m_CommandPermissionBuilder.GetPermission(commandContext.CommandRegistration);
                var permissionChecker = m_Runtime.Host.Services.GetRequiredService<IPermissionChecker>();

                if (!string.IsNullOrWhiteSpace(permission) && await permissionChecker.CheckPermissionAsync(actor, permission) != PermissionGrantResult.Grant)
                {
                    throw new NotEnoughPermissionException(stringLocalizer, permission);
                }

                Stopwatch sw = new Stopwatch();
                sw.Start();
                var command = commandContext.CommandRegistration.Instantiate(commandContext.ServiceProvider);
                await command.ExecuteAsync();
                m_Logger.LogDebug($"Command \"{string.Join(" ", args)}\" executed in {sw.ElapsedMilliseconds}ms");
                sw.Reset();

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
                        logger.LogError(commandContext.Exception, $"Exception occured on command \"{string.Join(" ", args)}\" by actor {actor.Type}/{actor.DisplayName} ({actor.Id})");
                    }
                }

                await commandContext.DisposeAsync();
            }

            return commandContext;
        }
    }
}