using System.IO;

namespace OpenMod.Common.Helpers
{
    internal static class StreamHelper
    {
        internal static byte[] ReadAllBytes(this Stream stream)
        {
            if (stream is MemoryStream memoryStream)
            {
                return memoryStream.ToArray();
            }

            using var newMemoryStream = new MemoryStream();
            stream.CopyTo(newMemoryStream);
            return newMemoryStream.ToArray();
        }
    }
}
