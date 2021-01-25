using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.API
{
    /// <summary>
    /// Represents a game or anything else that hosts OpenMod.
    /// </summary>
    [Service]
    public interface IOpenModHost : IOpenModComponent
    {
        /// <summary>
        /// Initializes the host.
        /// </summary>
        Task InitAsync();

        /// <summary>
        /// Shuts the host down gracefully.
        /// </summary>
        Task ShutdownAsync();

        /// <summary>
        /// Checks if the host has a capability.
        /// </summary>
        bool HasCapability(string capability);

        /// <summary>
        /// Hard reloads OpenMod binaries from disk. Used upgrading OpenMod without restarting.
        /// </summary>
        Task PerformHardReloadAsync();
    }
}