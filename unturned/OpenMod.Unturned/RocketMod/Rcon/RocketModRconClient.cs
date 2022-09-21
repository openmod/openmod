using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMod.Core.Helpers;
using OpenMod.Core.Rcon;
using OpenMod.Core.Rcon.Tcp;
using Rocket.Core;

namespace OpenMod.Unturned.RocketMod.Rcon
{
    public class RocketModRconClient : BaseTcpRconClient
    {
        private readonly ILogger<RocketModRconClient> m_Logger;
        private NewLineType? m_NewLineType;

        public RocketModRconClient(
            ILogger<RocketModRconClient> logger,
            TcpClient tcpClient,
            IRconHost host,
            IServiceProvider serviceProvider) : base(tcpClient, host, serviceProvider)
        {
            m_Logger = logger;
        }

        protected Task SendPacketAsync(RocketModRconPacket packet, CancellationToken cancellationToken = default)
        {
            //todo
            return SendDataAsync(packet.Serialize(), cancellationToken);
        }

        public override Task SendMessageAsync(string message, CancellationToken cancellationToken = default)
        {
            return SendPacketAsync(new RocketModRconPacket
            {
                Body = message,
                NewLineType = m_NewLineType ?? NewLineType.Windows
            }, cancellationToken);
        }

        protected override async Task OnDataReceivedAsync(ArraySegment<byte> data)
        {
            using var stream = new MemoryStream(data.Array!, data.Offset, data.Count);

            try
            {
                var packet = await RocketModRconPacket.ReadFromStreamAsync(stream);
                if (m_NewLineType == null && packet.Body != null)
                {
                    if (packet.Body.EndsWith("\r\n"))
                    {
                        m_NewLineType = NewLineType.Windows;
                        packet.Body = packet.Body[..^2];//lenght - 2
                    }
                    else if (packet.Body.EndsWith("\n"))
                    {
                        m_NewLineType = NewLineType.Linux;
                        packet.Body = packet.Body[..^1];//lenght - 1
                    }
                    else
                    {
                        m_NewLineType = NewLineType.Mac;
                        packet.Body = packet.Body[..^1];//lenght - 1
                    }
                }
                await ProcessPacketAsync(packet);
            }
            catch (Exception ex)
            {
                m_Logger.LogError(ex, "Error while procesing packet. Byte count: {count}", data.Count);
            }
        }

        protected virtual Task ProcessPacketAsync(RocketModRconPacket packet)
        {
            if (string.IsNullOrEmpty(packet.Body))
            {
                return Task.CompletedTask;
            }

            var arguments = packet.Body!.Split(' ');
            var command = arguments.Length == 1 ? packet.Body : arguments.First();
            if (command.Equals("login", StringComparison.InvariantCulture) && !IsAuthenticated)
            {
                return ProcessLoginAsync(packet);
            }

            return OnExecuteCommandAsync(packet.Body!);
        }

        private async Task ProcessLoginAsync(RocketModRconPacket packet)
        {
            var arguments = ArgumentsParser.ParseArguments(packet.Body!);

            var rocketModLoaded = RocketModIntegration.IsRocketModUnturnedLoaded(out _);
            if (rocketModLoaded && arguments.Length == 2)
            {
                if (ProcessRocketModLogin(packet))
                {
                    Id = "root";
                    IsAuthenticated = true;
                    await SendMessageAsync("Logged in.");
                    return;
                }

                await SendMessageAsync("Authentication failed.");

                IncreaseFailedAuthAttempts();
                await DisconnectOnFailedLoginAttemptsAsync();
                return;
            }

            if (arguments.Length != 3)
            {
                await SendMessageAsync("Invalid Syntax! Usage: login <username> <password>");

                IncreaseFailedAuthAttempts();
                await DisconnectOnFailedLoginAttemptsAsync();
                return;
            }

            await OnAuthenticatingAsync(arguments[1], arguments[2]);
        }

        private bool ProcessRocketModLogin(RocketModRconPacket packet)
        {
            var password = string.Join(" ", packet.Body!.Split(' ').Skip(1));
            var rocketPassword = R.Settings.Instance.RCON.Password.Trim();

            if (rocketPassword.Equals("changeme", StringComparison.Ordinal))
            {
                return false;
            }

            return string.Equals(password, rocketPassword, StringComparison.InvariantCulture);
        }
    }
}