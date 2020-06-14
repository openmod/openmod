using System.Reflection;
using Semver;

namespace OpenMod.Core.Helpers
{
    public static class VersionHelper
    {
        public static SemVersion ParseAssemblyVersion(Assembly assembly)
        {
            var version = assembly.GetName().Version;
            return new SemVersion(version.Major, version.Minor, version.Build, build: version.Revision != 0 ? version.Revision.ToString() : "");
        }
    }
}