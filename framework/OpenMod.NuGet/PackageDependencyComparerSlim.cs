using System;
using System.Collections.Generic;
using NuGet.Packaging.Core;

namespace OpenMod.NuGet
{
    internal sealed class PackageDependencyComparerSlim : IEqualityComparer<PackageDependency>
    {
        public static PackageDependencyComparerSlim Default { get; } = new();

        /// <inheritdoc />
        public bool Equals(PackageDependency? x, PackageDependency? y)
        {
            return string.Equals(x?.Id, y?.Id, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(PackageDependency? obj)
        {
            return obj?.Id.GetHashCode() ?? 0;
        }
    }
}
