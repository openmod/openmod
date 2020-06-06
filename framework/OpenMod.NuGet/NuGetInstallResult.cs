using NuGet.Packaging.Core;

namespace OpenMod.NuGet
{
    public sealed class NuGetInstallResult
    {
        public PackageIdentity Identity { get; }

        public NuGetInstallResult(NuGetInstallCode code)
        {
            Code = code;
        }

        public NuGetInstallResult(PackageIdentity identity)
        {
            Identity = identity;
            Code = NuGetInstallCode.Success;
        }

        public NuGetInstallCode Code { get; set; }
    }
}