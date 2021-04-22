using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMod.Core.Rcon.Tcp;

namespace OpenMod.Core.Rcon.Source
{
    public class SourceRconClient : BaseTcpRconClient
    {
        private readonly List<string> m_CommandMessageBuffer = new();
        private readonly ILogger<SourceRconClient> m_Logger;
        private int _authRequestId = -1;
        private int _latestCommandId;

        public SourceRconClient(
            TcpClient tcpClient,
            IRconHost host,
            IServiceProvider serviceProvider,
            ILogger<SourceRconClient> logger) : base(tcpClient, host, serviceProvider)
        {
            m_Logger = logger;
        }

        public override Task SendMessageAsync(string message, CancellationToken cancellationToken = default)
        {
            // Can not send messages directly, need to buffer it
            m_CommandMessageBuffer.Add(message);
            return Task.CompletedTask;
        }

        protected virtual Task SendMessageAsync(int requestId, string message, CancellationToken cancellationToken = default)
        {
            return SendPacketAsync(new SourceRconPacket
            {
                RequestId = requestId,
                Body = message,
                Type = SourceRconPacket.ServerDataResponsePacket
            }, cancellationToken);
        }

        protected virtual Task SendPacketAsync(SourceRconPacket packet, CancellationToken cancellationToken = default)
        {
            return SendDataAsync(packet.Serialize(), cancellationToken);
        }

        protected override async Task OnDataReceivedAsync(ArraySegment<byte> data)
        {
            using var stream = new MemoryStream(data.Array!, data.Offset, data.Count);

            try
            {
                var packets = await SourceRconPacket.ReadFromStreamAsync(stream);
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

        protected virtual async Task ProcessPacketAsync(SourceRconPacket packet)
        {
            switch (packet.Type)
            {
                case SourceRconPacket.ServerDataAuthPacket:
                    _authRequestId = packet.RequestId;

                    var line = packet.Body!;
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

                case SourceRconPacket.ServerDataExecuteCommandPacket:
                    if (string.IsNullOrEmpty(packet.Body))
                    {
                        break;
                    }

                    _latestCommandId = packet.RequestId;
                    await OnExecuteCommandAsync(packet.Body!);

                    var message = "Command executed.";
                    if (m_CommandMessageBuffer.Count > 0)
                    {
                        message = string.Join("\n", m_CommandMessageBuffer);
                        m_CommandMessageBuffer.Clear();
                    }

                    await SendMessageAsync(_latestCommandId, message);
                    break;

                default:
                    m_Logger.LogDebug("Received unknown packet: {PacketType}", packet.Type);
                    break;
            }
        }

        protected override async Task OnClientAuthenticationFailedAsync()
        {
            IncreaseFailedAuthAttempts();

            await SendPacketAsync(new SourceRconPacket
            {
                RequestId = _authRequestId,
                Type = SourceRconPacket.ServerDataResponsePacket
            });

            await SendPacketAsync(new SourceRconPacket
            {
                RequestId = -1,
                Type = SourceRconPacket.ServerDataAuthResponsePacket
            });
        }

        protected override async Task OnClientAuthenticatedAsync()
        {
            await SendPacketAsync(new SourceRconPacket
            {
                RequestId = _authRequestId,
                Type = SourceRconPacket.ServerDataResponsePacket
            });

            await SendPacketAsync(new SourceRconPacket
            {
                RequestId = _authRequestId,
                Type = SourceRconPacket.ServerDataAuthResponsePacket
            });

            _authRequestId = -1;
        }
    }
}