using System.Collections.Generic;

namespace OpenMod.NuGet
{
    public class SerializedNuGetPackage
    {
        public string Id { get; set; }
        public string Version { get; set; }

        public override bool Equals(object obj)
        {
            return obj is SerializedNuGetPackage package &&
                   Id == package.Id &&
                   Version == package.Version;
        }

        public override int GetHashCode()
        {
            var hashCode = -612338121;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Id);
            return hashCode;
        }
    }
}