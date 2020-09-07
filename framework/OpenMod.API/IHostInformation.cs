using Semver;

namespace OpenMod.API
{
    public interface IHostInformation
    {
        /// <summary>
        ///   Version of the OpenMod host implementation.
        /// </summary>
        SemVersion HostVersion { get; }

        /// <summary>
        ///    Name of the host implementation. E.g. OpenMod for Unturned
        /// </summary>
        string HostName { get; }
    }
}