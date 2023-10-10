using System;
using System.Collections.Generic;
using NuGet.Packaging.Core;

namespace OpenMod.NuGet;
internal sealed class PackageDependencyComparerSlim : IEqualityComparer<PackageDependency>
{
    public static PackageDependencyComparerSlim Default { get; } = new();

    public bool Equals(PackageDependency x, PackageDependency y)
    {
        if (x == y)
        {
            return true;
        }

        return x != null && y != null && x.Id.Equals(y.Id, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(PackageDependency obj)
    {
        return obj?.Id.GetHashCode() ?? 0;
    }
}
