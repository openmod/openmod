using System;
using NuGet.Packaging.Core;

namespace OpenMod.NuGet
{
    public sealed class NuGetAssembly
    {
        public string AssemblyName { get; set; } = null!;
        public Version Version { get; set; } = null!;
        public WeakReference Assembly { get; set; } = null!;
        public PackageIdentity Package { get; set; } = null!;
    }
}