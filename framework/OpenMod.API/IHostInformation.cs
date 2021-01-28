using Semver;

namespace OpenMod.API
{
    /// <summary>
    /// Provides information about the OpenMod host.
    /// </summary>
    public interface IHostInformation
    {
        /// <summary>
        /// Gets the version of the OpenMod host implementation.
        /// </summary>
        SemVersion HostVersion { get; }

        /// <summary>
        /// Gets the name of the host implementation.
        /// </summary>
        /// <example>
        /// OpenMod for Unturned
        /// </example>
        string HostName { get; }
    }
}