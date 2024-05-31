using System.Collections.Generic;
using VYaml.Annotations;

namespace OpenMod.NuGet
{
    [YamlObject]
    public partial class SerializedPackagesFile
    {
        public HashSet<SerializedNuGetPackage>? Packages { get; set; }
    }
}