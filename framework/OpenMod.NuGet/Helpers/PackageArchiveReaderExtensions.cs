using NuGet.Packaging;
using OpenMod.Common.Helpers;

namespace OpenMod.NuGet.Helpers
{
    internal static class PackageArchiveReaderExtensions
    {
        public static byte[] ReadAllBytes(this PackageArchiveReader packageReader, string item)
        {
            var entry = packageReader.GetEntry(item);
            using var stream = entry.Open();
            return stream.ReadAllBytes();
        }
    }
}
