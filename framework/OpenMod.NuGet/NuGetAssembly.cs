using System;
using NuGet.Packaging.Core;

namespace OpenMod.NuGet
{
    public sealed class NuGetAssembly
    {
        public string AssemblyName { get; set; }
        public Version Version { get; set; }
        public WeakReference Assembly { get; set; }
        public PackageIdentity Package { get; set; }
    }
}