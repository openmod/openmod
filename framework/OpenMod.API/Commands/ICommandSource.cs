using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

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
        /// <returns>Commands of this provider. Cannot return null and items can not be null.</returns>
        [ItemNotNull]
        [NotNull]
        Task<IReadOnlyCollection<ICommandRegistration>> GetCommandsAsync();
    }
}