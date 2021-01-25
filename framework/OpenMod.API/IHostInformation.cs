using JetBrains.Annotations;
using Semver;

namespace OpenMod.API
{
    /// <summary>
    /// Provides information about the OpenMod host.
    /// </summary>
    public interface IHostInformation
    {
        /// <value>
        /// The version of the OpenMod host implementation. Cannot be null.
        /// </value>
        [NotNull]
        SemVersion HostVersion { get; }

        /// <value>
        /// The Name of the host implementation. Cannot be null or empty.
        /// </value>
        /// <example>
        /// OpenMod for Unturned
        /// </example>
        [NotNull]
        string HostName { get; }
    }
}