using Semver;

namespace OpenMod.API
{
    /// <summary>
    /// Provides information about the OpenMod host.
    /// </summary>
    public interface IHostInformation
    {
        /// <value>
        /// The version of the OpenMod host implementation.
        /// </value>
        SemVersion HostVersion { get; }

        /// <value>
        /// The Name of the host implementation.
        /// </value>
        /// <example>
        /// OpenMod for Unturned
        /// </example>
        string HostName { get; }
    }
}