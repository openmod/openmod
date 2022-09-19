using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace OpenMod.Unturned.RocketMod.Rcon
{
    public class RocketModRconPacket
    {
        public string? Body { get; set; }
        public NewLineType NewLineType { get; set; }

        public static async Task<RocketModRconPacket> ReadFromStreamAsync(Stream stream)
        {
            var buffer = new byte[stream.Length];
            _ = await stream.ReadAsync(buffer, 0, buffer.Length);

            return new RocketModRconPacket
            {
                Body = Encoding.UTF8.GetString(buffer)
            };
        }

        public byte[] Serialize()
        {
            var endLine = NewLineType switch
            {
                NewLineType.Windows => "\r\n",
                NewLineType.Linux => "\n",
                NewLineType.Mac => "\r",
                _ => "\n"
            };
            return Encoding.UTF8.GetBytes(Body + endLine);
        }
    }
}