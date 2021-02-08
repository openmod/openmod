using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenMod.Core.Rcon.Source
{
    public class SourceRconPacket
    {
        public const string ServerDataAuthPacket = "SERVERDATA_AUTH";
        public const string ServerDataAuthResponsePacket = "SERVERDATA_AUTH_RESPONSE";
        public const string ServerDataExecuteCommandPacket = "SERVERDATA_EXECCOMMAND";
        public const string ServerDataResponsePacket = "SERVERDATA_RESPONSE_VALUE";

        public int RequestId { get; set; }
        public string? Type { get; set; }
        public string? Body { get; set; }

        private static readonly IDictionary<string, int> s_PacketTypes = new Dictionary<string, int>
        {
            [ServerDataResponsePacket] = 0,
            [ServerDataExecuteCommandPacket] = 2,
            [ServerDataAuthResponsePacket] = 2,
            [ServerDataAuthPacket] = 3
        };

        public static async Task<List<SourceRconPacket>> ReadFromStreamAsync(Stream stream)
        {
            var packets = new List<SourceRconPacket>();

            while (true)
            {
                try
                {
                    var packet = new SourceRconPacket();
                    var sizeBuffer = new byte[sizeof(int)];
                    var read = await stream.ReadAsync(sizeBuffer, 0, sizeof(int));
                    if (read == 0)
                    {
                        break;
                    }

                    var size = BitConverter.ToInt32(sizeBuffer, 0);

                    var packetBuffer = new byte[size];
                    await stream.ReadAsync(packetBuffer, 0, packetBuffer.Length);

                    packet.RequestId = BitConverter.ToInt32(packetBuffer, 0);
                    if (!TryGetNativePacketTypeIdentifier(BitConverter.ToInt32(packetBuffer, sizeof(int)),
                        out var packetType))
                    {
                        throw new ArgumentException($"Packet contained unknown packet type!");
                    }

                    packet.Type = packetType;
                    packet.Body = Encoding.ASCII.GetString(packetBuffer, sizeof(int) * 2, packetBuffer.Length - sizeof(int) * 2 - sizeof(byte) * 2);
                    packets.Add(packet);
                }
                catch (IOException)
                {
                    break;
                }
            }

            return packets;
        }

        public byte[] Serialize()
        {
            if (Type == null)
            {
                throw new Exception("Packet Type not set");
            }

            if (!TryGetPacketTypeIdentifier(Type, out var typeId))
            {
                throw new ArgumentException($"Type: {Type} not recognized!");
            }

            var body = Encoding.UTF8.GetBytes(Body + "\0"); // add null string terminator
            var buffer = new byte[sizeof(int) * 3 + body.Length + 1]; // 12 bytes for Length, Id and Type

            BitConverter.GetBytes(buffer.Length - sizeof(int)).CopyTo(buffer, 0); //Size 
            BitConverter.GetBytes(RequestId).CopyTo(buffer, sizeof(int));
            BitConverter.GetBytes(typeId).CopyTo(buffer, sizeof(int) * 2);
            body.CopyTo(buffer, sizeof(int) * 3);

            return buffer;
        }

        private static bool TryGetNativePacketTypeIdentifier(int num, out string? type)
        {
            type = null;

            if (s_PacketTypes.All(c => c.Value != num))
            {
                return false;
            }

            type = s_PacketTypes.FirstOrDefault(c => c.Value == num).Key;
            return true;
        }

        private bool TryGetPacketTypeIdentifier(string type, out int result)
        {
            return s_PacketTypes.TryGetValue(type, out result);
        }
    }
}