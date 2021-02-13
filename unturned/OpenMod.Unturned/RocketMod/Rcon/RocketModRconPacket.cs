using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace OpenMod.Unturned.RocketMod.Rcon
{
    public class RocketModRconPacket
    {
        public string? Body { get; set; }

        public static async Task<RocketModRconPacket> ReadFromStreamAsync(Stream stream)
        {
            var buffer = new byte[stream.Length];
            await stream.ReadAsync(buffer, 0, buffer.Length);

            return new RocketModRconPacket
            {
                Body = Encoding.UTF8.GetString(buffer, 0, buffer.Length - 2)
            };
        }

        public byte[] Serialize()
        {
            return Encoding.UTF8.GetBytes(Body + "\r\n");
        }
    }
}