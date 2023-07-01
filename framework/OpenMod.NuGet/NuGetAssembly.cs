using System;
using System.Reflection;
using NuGet.Packaging.Core;

namespace OpenMod.NuGet
{
    public sealed class NuGetAssembly
    {
        [Obsolete("Use " + nameof(AssemblyName2) + ".Name")]
        public string AssemblyName
        {
            get { return AssemblyName2.Name; }
            set { /* ignore set */ }
        }
        public AssemblyName AssemblyName2 { get; set; } = null!;

        [Obsolete("Use " + nameof(AssemblyName2) + ".Version")]
        public Version Version
        {
            get { return AssemblyName2.Version; }
            set { /* ignore set */ }
        }

        public WeakReference Assembly { get; set; } = null!;
        public PackageIdentity Package { get; set; } = null!;
    }
}