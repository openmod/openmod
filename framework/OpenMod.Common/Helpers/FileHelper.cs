using System.IO;
using System.Threading.Tasks;

namespace OpenMod.Common.Helpers
{
    internal static class FileHelper
    {
        internal static async Task<byte[]> ReadAllBytesAsync(string path)
        {
            using var stream = File.Open(path, FileMode.Open);
            var buffer = new byte[stream.Length];
            _ = await stream.ReadAsync(buffer, offset: 0, count: (int) stream.Length);
            return buffer;
        }
    }
}
