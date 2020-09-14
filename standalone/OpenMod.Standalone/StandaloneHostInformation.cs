using OpenMod.API;
using OpenMod.Core.Helpers;
using Semver;

namespace OpenMod.Standalone
{
    public class StandaloneHostInformation : IHostInformation
    {
        public StandaloneHostInformation()
        {
            HostVersion = VersionHelper.ParseAssemblyVersion(GetType().Assembly);
        }
        public SemVersion HostVersion { get; }

        public string HostName => "Openmod Standalone";
    }
}
