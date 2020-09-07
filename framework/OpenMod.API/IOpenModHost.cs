using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.API
{
    /// <summary>
    ///    Represents a game or anything else that hosts OpenMod
    /// </summary>
    [Service]
    public interface IOpenModHost : IOpenModComponent
    {
        /// <summary>
        ///     Initializes the host
        /// </summary>
        /// <returns></returns>
        Task InitAsync();

        /// <summary>
        ///    Gracefully exits the host.
        /// </summary>
        Task ShutdownAsync();

        /// <summary>
        ///    Checks if the host has a specific capability (e.g. supports inventories for games)
        /// </summary>

        bool HasCapability(string capability);
    }
}