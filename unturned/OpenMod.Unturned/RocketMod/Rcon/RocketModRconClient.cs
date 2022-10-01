using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMod.Core.Helpers;
using OpenMod.Core.Rcon;
using OpenMod.Core.Rcon.Tcp;
using Rocket.Core;

namespace OpenMod.Unturned.RocketMod.Rcon
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RocketModRconClient : BaseTcpRconClient
    {
        private readonly List<byte> m_Buffer;
        private readonly ILogger<RocketModRconClient> m_Logger;
        private NewLineType? m_NewLineType;

        public RocketModRconClient(
            ILogger<RocketModRconClient> logger,
            TcpClient tcpClient,
            IRconHost host,
            IServiceProvider serviceProvider) : base(tcpClient, host, serviceProvider)
        {
            m_Logger = logger;
            m_Buffer = new List<byte>();
        }

        // ReSharper disable once MemberCanBePrivate.Global
        protected Task SendPacketAsync(RocketModRconPacket packet, CancellationToken cancellationToken = default)
        {
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
            m_Logger.LogDebug("Current buffer({BufferCount}): {Str}", m_Buffer.Count, BitConverter.ToString(m_Buffer.ToArray()));
            m_Logger.LogDebug("Data receiived({DataCount}): {Str}", data.Count, BitConverter.ToString(data.ToArray()));

            for (var i = data.Offset; i < data.Count; i++)
            {
                try
                {
                    var bt = data.Array![i];
                    switch (bt)
                    {
                        //putty negotiation FF XX XX, FF ...
                        case 0xFF:
                            i += 2;
                            continue;

                        case 0x0D: //check if nex char is \n
                            if (++i >= data.Count)
                            {
                                m_NewLineType ??= NewLineType.Mac;
                                break;
                            }

                            var nextBt = data.Array[i];
                            if (nextBt is not 0x0A)
                            {
                                m_NewLineType ??= NewLineType.Mac;
                                i--;
                                break;
                            }

                            m_NewLineType ??= NewLineType.Windows;
                            break;

                        case 0x0A:
                            m_NewLineType ??= NewLineType.Linux;
                            break;

                        default:
                            m_Buffer.Add(bt);
                            continue;
                    }
                }
                catch (Exception ex)
                {
                    m_Logger.LogError(ex, "Error while reading data received. Byte count: {DataCount} | Index: {I}",
                        data.Count, i);
                    continue;
                }

                try
                {
                    if (m_Buffer.Count == 0)
                    {
                        if (!IsAuthenticated)
                        {
                            await OnExecuteCommandAsync("openmod");
                        }
                        continue;
                    }

                    //Buffer received a msg
                    var packet = new RocketModRconPacket
                    {
                        Body = Encoding.UTF8.GetString(m_Buffer.ToArray())
                    };
                    m_Buffer.Clear();

                    await ProcessPacketAsync(packet);
                }
                catch (Exception ex)
                {
                    m_Logger.LogError(ex, "Error while creating rocketmod rocn packet: {BufferCount}", m_Buffer.Count);
                }
            }
        }

        protected virtual Task ProcessPacketAsync(RocketModRconPacket packet)
        {
            try
            {
                if (string.IsNullOrEmpty(packet.Body)) return Task.CompletedTask;

                var arguments = packet.Body!.Split(' ');
                var command = arguments.Length == 1 ? packet.Body : arguments.First();
                if (command.Equals("login", StringComparison.InvariantCulture) && !IsAuthenticated)
                    return ProcessLoginAsync(packet);

                return OnExecuteCommandAsync(packet.Body!);
            }
            catch (Exception ex)
            {
                m_Logger.LogError(ex, "Error while procesing packet. Byte count: {Count}", packet.Body?.Length ?? -1);
                return Task.FromException(ex);
            }
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
            var password = string.Join(" ", packet.Body!.Split(' ').Skip(count: 1));
            var rocketPassword = R.Settings.Instance.RCON.Password.Trim();

            if (rocketPassword.Equals("changeme", StringComparison.Ordinal)) return false;

            return string.Equals(password, rocketPassword, StringComparison.InvariantCulture);
        }

        public override ValueTask DisposeAsync(CancellationToken cancellationToken)
        {
            m_Buffer.Clear();
            m_NewLineType = null;
            return base.DisposeAsync(cancellationToken);
        }
    }
}