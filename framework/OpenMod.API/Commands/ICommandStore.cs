using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
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
        /// Gets the commands of all registered <see cref="ICommandSource"/>s. Can not return null or null items.
        /// </summary>
        /// <returns></returns>
        [ItemNotNull]
        Task<IReadOnlyCollection<ICommandRegistration>> GetCommandsAsync();

        /// <summary>
        /// Refreshes the cache. Must be used if a command source has added or removed commands.
        /// </summary>
        Task InvalidateAsync();
    }
}