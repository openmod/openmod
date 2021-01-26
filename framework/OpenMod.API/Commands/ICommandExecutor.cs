using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.API.Commands
{
    /// <summary>
    /// The service for executing commands.
    /// </summary>
    [Service]
    public interface ICommandExecutor
    {
        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="actor">The command actor.</param>
        /// <param name="args">The command args.</param>
        /// <param name="prefix">The command prefix. Can be empty.</param>
        /// <returns>The created command context.</returns>
        Task<ICommandContext> ExecuteAsync(ICommandActor actor, string[] args, string prefix);
    }
}