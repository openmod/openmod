using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OpenMod.Core.Rcon.Minecraft
{
    public class MinecraftRconPacket
    {
        public int RequestId { get; set; }

        public MinecraftPacketType Type { get; set; }

        public byte[]? Payload { get; set; }

        public static async Task<List<MinecraftRconPacket>> ReadFromStreamAsync(Stream stream)
        {
            var packets = new List<MinecraftRconPacket>();

            while (true)
            {
                try
                {
                    var packet = new MinecraftRconPacket();
                    var sizeBuffer = new byte[sizeof(int)];
                    var read = await stream.ReadAsync(sizeBuffer, 0, sizeof(int));
                    if (read == 0)
                    {
                        break;
                    }

                    var size = BitConverter.ToInt32(sizeBuffer, 0);

                    var packetBuffer = new byte[size];
                    await stream.ReadAsync(packetBuffer, 0, packetBuffer.Length);

                    packet.RequestId = FromLittleEndian(packetBuffer, 0 * sizeof(int));
                    packet.Type = (MinecraftPacketType) FromLittleEndian(packetBuffer, 1 * sizeof(int));

                    // 8 bytes: RequestId + Payload
                    // 1 byte padding
                    if (size > 9)
                    {
                        // ignore 1 additional byte for null terminator
                        packet.Payload = new byte[size - 9 - 1];
                        Array.Copy(packetBuffer!, 8, packet.Payload, 0, packet.Payload.Length);
                    }

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
            using var ms = new MemoryStream();
            var length = sizeof(int) + sizeof(int) + (Payload?.Length ?? 0) + sizeof(byte);
            ms.Write(ToLittleEndian(length), 0, sizeof(int));
            ms.Write(ToLittleEndian(RequestId), 0, sizeof(int));
            ms.Write(ToLittleEndian((int)Type), 0, sizeof(int));

            if (Payload != null)
            {
                ms.Write(Payload, 0, Payload.Length);
            }

            // padding
            ms.WriteByte(0x00);
            return ms.ToArray();
        }

        private static byte[] ToLittleEndian(int data)
        {
            byte[] b = new byte[4];
            b[0] = (byte)data;
            b[1] = (byte)(((uint)data >> 8) & 0xFF);
            b[2] = (byte)(((uint)data >> 16) & 0xFF);
            b[3] = (byte)(((uint)data >> 24) & 0xFF);
            return b;
        }

        private static int FromLittleEndian(byte[] data, int startIndex)
        {
            return (data[startIndex + 3] << 24)
                   | (data[startIndex + 2] << 16)
                   | (data[startIndex + 1] << 8)
                   | data[startIndex];
        }
    }

    public enum MinecraftPacketType
    {
        Response = 0,
        Command = 2,
        Login = 3
    }
}