using OpenMod.API.Ioc;

namespace OpenMod.API.Commands
{
    /// <summary>
    /// The service for getting the current command context.
    /// </summary>
    [Service]
    public interface ICurrentCommandContextAccessor
    {
        /// <summary>
        /// Gets or sets the command context processed by the current thread. Returns null if the current thread does not handle a command currently.
        /// </summary>
        ICommandContext? Context { get; set; }
    }
}