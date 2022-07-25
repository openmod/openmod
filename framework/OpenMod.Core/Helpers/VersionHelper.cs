using System.Reflection;
using OpenMod.API;
using Semver;

namespace OpenMod.Core.Helpers
{
    [OpenModInternal]
    public static class VersionHelper
    {
        public static SemVersion ParseAssemblyVersion(Assembly assembly)
        {
            var informationalAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if(informationalAttribute != null)
            {
                return SemVersion.Parse(informationalAttribute.InformationalVersion, SemVersionStyles.Any);
            }

            var version = assembly.GetName().Version;
            return SemVersion.ParsedFrom(version.Major, version.Minor, version.Build, prerelease: version.Revision != 0 ? version.Revision.ToString() : string.Empty);
        }
    }
}