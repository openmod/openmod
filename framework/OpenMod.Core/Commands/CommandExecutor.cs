using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenMod.API.Commands;
using OpenMod.API.Ioc;
using OpenMod.API.Localization;
using OpenMod.API.Permissions;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Commands
{
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Transient, Priority = Priority.Lowest)]
    public class CommandExecutor : ICommandExecutor
    {
        private readonly ILifetimeScope m_LifetimeScope;
        private readonly IOptions<CommandExecutorOptions> m_CommandExecutorOptions;

        public CommandExecutor(
            ILifetimeScope lifetimeScope,
            IOptions<CommandExecutorOptions> commandExecutorOptions)
        {
            m_LifetimeScope = lifetimeScope;
            m_CommandExecutorOptions = commandExecutorOptions;
        }
        public async Task<ICommandContext> ExecuteAsync(ICommandActor actor, string[] args, string prefix)
        {
            if (args == null || args.Length == 0)
            {
                throw new Exception("Can not execute command with null or empty args");
            }

            var currentCommandAccessor = m_LifetimeScope.Resolve<ICurrentCommandContextAccessor>();
            var serviceProvider = m_LifetimeScope.Resolve<IServiceProvider>();
            var commandSources = m_CommandExecutorOptions.Value.CreateCommandSources(serviceProvider);
            var commandsRegistrations = commandSources.SelectMany(d => d.Commands).ToList();
            var logger = m_LifetimeScope.Resolve<ILogger<CommandExecutor>>();
            var commandContextBuilder = m_LifetimeScope.Resolve<ICommandContextBuilder>();
            var permissionChecker = m_LifetimeScope.Resolve<IPermissionChecker>();
            var stringLocalizer = m_LifetimeScope.Resolve<IOpenModStringLocalizer>();
            var commandContext = commandContextBuilder.CreateContext(actor, args, prefix, commandsRegistrations);

            logger.LogInformation($"Actor {actor.Type}/{actor.DisplayName} ({actor.Id}) has executed command \"{string.Join(" ", args)}\".");

            try
            {
                if (commandContext.Exception != null)
                {
                    throw commandContext.Exception;
                }

                currentCommandAccessor.Context = commandContext;
                // todo: permissions don't work, they cause exceptions. Also it calls the wrong PermissionCheckProvider
                // var permission = commandContext.CommandRegistration.Permission;
                //if (!string.IsNullOrWhiteSpace(permission) && await permissionChecker.CheckPermissionAsync(actor, permission) != PermissionGrantResult.Grant)
                //{
                //    throw new NotEnoughPermissionException(permission, stringLocalizer);
                //}

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
            }
            finally
            {
                await commandContext.DisposeAsync();
            }

            return commandContext;
        }
    }
}