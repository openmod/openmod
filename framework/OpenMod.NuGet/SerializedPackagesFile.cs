using System.Collections.Generic;

namespace OpenMod.NuGet
{
    public class SerializedPackagesFile
    {
        public HashSet<SerializedNuGetPackage>? Packages { get; set; }
    }
}