using JetBrains.Annotations;
using OpenMod.API.Ioc;

namespace OpenMod.API.Commands
{
    /// <summary>
    /// The service for getting the current command context.
    /// </summary>
    [Service]
    public interface ICurrentCommandContextAccessor
    {
        /// <value>
        /// The command context processed by the current thread. Can be null if the current thread does not handle a command.
        /// </value>
        [CanBeNull]
        ICommandContext Context { get; set; }
    }
}