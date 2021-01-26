using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenMod.API.Commands
{
    /// <summary>
    /// Represents a command source that provides commands.
    /// </summary>
    public interface ICommandSource
    {
        /// <summary>
        /// Gets the commands from the command source.
        /// </summary>
        /// <returns>Commands of this provider.</returns>
        Task<IReadOnlyCollection<ICommandRegistration>> GetCommandsAsync();
    }
}