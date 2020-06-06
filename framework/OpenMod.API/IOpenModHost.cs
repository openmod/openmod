using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.API
{
    /// <summary>
    ///    Represents a game or anything else that hosts OpenMod
    /// </summary>
    [Service]
    public interface IOpenModHost
    {
        /// <summary>
        ///     Initializes the host
        /// </summary>
        /// <returns></returns>
        Task InitAsync();

        /// <summary>
        ///     The name of the host. E.g. the game's name.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///    The version of the host. Version format depends on the host.
        /// </summary>
        string Version { get; }
    }
}