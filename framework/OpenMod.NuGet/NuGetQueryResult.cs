using System;
using System.Collections.Generic;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;

namespace OpenMod.NuGet
{
    public class NuGetQueryResult
    {
        public NuGetQueryResult(NuGetInstallCode code)
        {
            Code = code;
            Packages = null;
        }

        public NuGetQueryResult(ICollection<SourcePackageDependencyInfo> packages)
        {
            Packages = packages ?? throw new ArgumentNullException(nameof(packages));
            Code = NuGetInstallCode.Success;
        }

        public NuGetQueryResult(PackageIdentity identity, NuGetInstallCode code)
        {
            InstalledPackage = identity;
            Code = code;
        }

        public NuGetInstallCode Code { get; }

        public ICollection<SourcePackageDependencyInfo>? Packages { get; }

        public PackageIdentity? InstalledPackage { get; }
    }
}