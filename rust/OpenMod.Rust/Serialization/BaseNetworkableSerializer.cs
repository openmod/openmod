using System.IO;

namespace OpenMod.Rust.Serialization
{
    public static class BaseNetworkableSerializer
    {
        public static byte[] Serialize(BaseNetworkable networkable, out BaseNetworkable.SaveInfo saveInfo)
        {
            saveInfo = new BaseNetworkable.SaveInfo
            {
                forConnection = networkable.net.connection,
                forDisk = true
            };

            using var ms = new MemoryStream();
            networkable.ToStreamForNetwork(ms, saveInfo);
            ms.Seek(offset: 0, SeekOrigin.Begin);
            return ms.ToArray();
        }
    }
}