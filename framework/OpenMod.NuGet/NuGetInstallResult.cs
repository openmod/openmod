using System;
using NuGet.Packaging.Core;

namespace OpenMod.NuGet
{
    public sealed class NuGetInstallResult
    {
        public PackageIdentity? Identity { get; }

        public NuGetInstallResult(NuGetInstallCode code)
        {
            Code = code;
        }

        public NuGetInstallResult(PackageIdentity identity)
        {
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            Code = NuGetInstallCode.Success;
        }

        public NuGetInstallResult(NuGetInstallCode code, PackageIdentity? identity)
        {
            Identity = identity;
            Code = code;
        }


        public NuGetInstallCode Code { get; set; }
    }
}