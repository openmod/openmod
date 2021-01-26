using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.API.Commands
{
    /// <summary>
    /// The service for getting commands.
    /// </summary>
    [Service]
    public interface ICommandStore
    {
        /// <summary>
        /// Gets the commands of all registered <see cref="ICommandSource"/>s.
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyCollection<ICommandRegistration>> GetCommandsAsync();

        /// <summary>
        /// Refreshes the cache. Must be used if a command source has added or removed commands.
        /// </summary>
        Task InvalidateAsync();
    }
}