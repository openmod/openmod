using System;
using System.Reflection;

namespace OpenMod.NuGet
{
    public sealed class CachedNuGetAssembly
    {
        public string AssemblyName { get; set; }
        public Version Version { get; set; }
        public Assembly Assembly { get; set; }
    }
}