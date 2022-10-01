using System.Text;

namespace OpenMod.Unturned.RocketMod.Rcon
{
    public class RocketModRconPacket
    {
        public string? Body { get; set; }
        public NewLineType NewLineType { get; set; }

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