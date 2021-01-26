using System.Collections.Generic;
using OpenMod.API.Ioc;

namespace OpenMod.API.Commands
{
    /// <summary>
    /// The service for creating command contexts.
    /// </summary>
    [Service]
    public interface ICommandContextBuilder
    {
        /// <summary>
        /// Creates a command context.
        /// </summary>
        /// <param name="actor">The command actor.</param>
        /// <param name="args">The arguments for the command.</param>
        /// <param name="prefix">The prefix for the command. Can be empty.</param>
        /// <param name="commandRegistrations">The command registrations used for looking up commands and their child commands. See <see cref="ICommandStore.GetCommandsAsync"/>.</param>
        /// <returns>The created command context.</returns>
        ICommandContext CreateContext(ICommandActor actor, string[] args, string prefix, IReadOnlyCollection<ICommandRegistration> commandRegistrations);
    }
}