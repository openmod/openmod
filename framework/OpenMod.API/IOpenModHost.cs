using System.Threading.Tasks;
using OpenMod.API.Ioc;
using Semver;

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
        ///     The name of the host. E.g. the game's name.
        /// </summary>
        string HostDisplayName { get; }

        /// <summary>
        ///    The version of the host. E.g. the game's version.
        /// </summary>
        string HostVersion { get; }

        /// <summary>
        ///   Version of the OpenMod host implementation.
        /// </summary>
        SemVersion Version { get; }

        /// <summary>
        ///    Name of the host implementation. E.g. OpenMod for Unturned
        /// </summary>
        string Name { get; }
    }
}