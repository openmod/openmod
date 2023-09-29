using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MoreLinq;
using OpenMod.Core.Rcon.Tcp;

namespace OpenMod.Core.Rcon.Minecraft
{
    public class MinecraftRconClient : BaseTcpRconClient
    {
        private readonly List<string> m_CommandMessageBuffer = new();
        private readonly ILogger<MinecraftRconClient> m_Logger;
        private readonly Encoding m_Encoding;
        private int m_LoginRequestId = -1;
        private int m_LatestCommandId = -1;


        public MinecraftRconClient(
            TcpClient tcpClient,
            IRconHost host,
            ILogger<MinecraftRconClient> logger,
            IConfiguration configuration,
            IServiceProvider serviceProvider) : base(tcpClient, host, serviceProvider)
        {
            m_Logger = logger;

            m_Encoding =
                Encoding.GetEncodings()
                    .FirstOrDefault(d => d.Name.Equals(configuration["rcon:minecraft:encoding"] ?? "UTF8", StringComparison.OrdinalIgnoreCase))?
                    .GetEncoding()
                ?? Encoding.UTF8;
        }

        public override Task SendMessageAsync(string message, CancellationToken cancellationToken = default)
        {
            // Can not send messages directly, need to buffer it
            m_CommandMessageBuffer.Add(message);
            return Task.CompletedTask;
        }

        protected virtual Task SendMessageAsync(int requestId, string message, CancellationToken cancellationToken = default)
        {
            var packet = new MinecraftRconPacket
            {
                RequestId = requestId,
                Type = MinecraftPacketType.Response,
                Payload = m_Encoding.GetBytes(message + '\0')
            };

            return SendPacketAsync(packet, cancellationToken);
        }

        protected virtual Task SendPacketAsync(MinecraftRconPacket packet, CancellationToken cancellationToken = default)
        {
            return SendDataAsync(packet.Serialize(), cancellationToken);
        }

        protected override async Task OnDataReceivedAsync(ArraySegment<byte> data)
        {
            using var stream = new MemoryStream(data.Array!, data.Offset, data.Count);

            try
            {
                var packets = await MinecraftRconPacket.ReadFromStreamAsync(stream);
                foreach (var packet in packets)
                {
                    await ProcessPacketAsync(packet);
                }
            }
            catch (Exception ex)
            {
                m_Logger.LogError(ex, "Error while procesing packet. Byte count: {count}", data.Count);
            }
        }

        private async Task ProcessPacketAsync(MinecraftRconPacket packet)
        {
            if (packet.Payload == null || packet.Payload.Length == 0)
            {
                return;
            }

            switch (packet.Type)
            {
                case MinecraftPacketType.Command:
                    m_LatestCommandId = packet.RequestId;
                    var command = m_Encoding.GetString(packet.Payload!);
                    await OnExecuteCommandAsync(command);

                    var message = "Command executed.";
                    if (m_CommandMessageBuffer.Count > 0)
                    {
                        message = string.Join("\n", m_CommandMessageBuffer);
                        m_CommandMessageBuffer.Clear();
                    }

                    await SendMessageAsync(m_LatestCommandId, message);
                    break;

                case MinecraftPacketType.Login:
                    m_LoginRequestId = packet.RequestId;

                    var line = m_Encoding.GetString(packet.Payload!);
                    if (!line.Contains(":"))
                    {
                        await OnClientAuthenticationFailedAsync();
                        return;
                    }

                    var parts = line.Split(':');
                    var username = parts[0];
                    var password = string.Join(":", parts.Skip(1));
                    await OnAuthenticatingAsync(username, password);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override async Task OnClientAuthenticatedAsync()
        {
            await SendMessageAsync(m_LoginRequestId, $"Logged in as {Id}.");
            m_LoginRequestId = -1;
        }

        protected override async Task OnClientAuthenticationFailedAsync()
        {
            IncreaseFailedAuthAttempts();

            await SendPacketAsync(new MinecraftRconPacket
            {
                RequestId = -1,
                Type = MinecraftPacketType.Response
            });
        }
    }
}