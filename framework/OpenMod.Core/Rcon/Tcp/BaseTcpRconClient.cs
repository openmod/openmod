using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Eventing;
using OpenMod.Core.Helpers;
using OpenMod.Core.Rcon.Events;
using OpenMod.Core.Users;

namespace OpenMod.Core.Rcon.Tcp
{
    public abstract class BaseTcpRconClient : IAsyncDisposable, IRconClient
    {
        private readonly IEventBus m_EventBus;
        private readonly IRuntime m_Runtime;
        private readonly ICommandExecutor m_CommandExecutor;
        private readonly IConfiguration m_OpenModConfiguration;
        private readonly ILogger<BaseTcpRconClient> m_Logger;

        private NetworkStream? m_Stream;
        private Task? m_ListenerTask;
        private bool m_IsDisposed;

        protected BaseTcpRconClient(
            TcpClient tcpClient,
            IRconHost host,
            IServiceProvider serviceProvider)
        {
            m_EventBus = serviceProvider.GetRequiredService<IEventBus>();
            m_Runtime = serviceProvider.GetRequiredService<IRuntime>();
            m_OpenModConfiguration = serviceProvider.GetRequiredService<IConfiguration>();
            m_CommandExecutor = serviceProvider.GetRequiredService<ICommandExecutor>();
            m_Logger = serviceProvider.GetRequiredService<ILogger<BaseTcpRconClient>>();

            Host = host;
            TcpClient = tcpClient ?? throw new ArgumentNullException(nameof(tcpClient));
        }

        public virtual string Id { get; protected set; } = "Unauthenticated";

        public string Type { get; } = KnownActorTypes.Rcon;

        public virtual string DisplayName
        {
            get { return Id; }
        }

        public virtual string FullActorName
        {
            get { return $"{DisplayName} (RconConnection@{EndPoint})"; }
        }

        public TcpClient TcpClient { get; set; }

        public IRconHost Host { get; }

        public EndPoint EndPoint
        {
            get { return TcpClient.Client.RemoteEndPoint; }
        }

        public bool IsConnected
        {
            get { return TcpClient.Client.Connected; }
        }

        private int m_FailedAuthAttempts = 0;
        public bool IsAuthenticated { get; protected set; }

        public virtual Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (m_Stream != null)
            {
                return Task.CompletedTask;
            }

            if (TcpClient == null)
            {
                throw new ObjectDisposedException(nameof(TcpClient));
            }

            m_Stream = TcpClient.GetStream();

            m_ListenerTask = Task.Run(async () =>
            {
                try
                {
                    var buffer = new byte[TcpClient.ReceiveBufferSize];

                    while (true)
                    {
                        var closeReason = ConnectionCloseReason.Unknown;

                        int readLength;
                        try
                        {
                            readLength = await m_Stream.ReadAsync(buffer, 0, TcpClient.ReceiveBufferSize, cancellationToken);
                        }
                        catch (IOException ex) when ((ex.InnerException as SocketException)?.ErrorCode ==
                                                     (int)SocketError.OperationAborted ||
                                                     (ex.InnerException as SocketException)?.ErrorCode == 125)
                        {
                            closeReason = ConnectionCloseReason.Closed;
                            readLength = -1;
                        }
                        catch (IOException ex) when ((ex.InnerException as SocketException)?.ErrorCode ==
                                                     (int)SocketError.ConnectionAborted)
                        {
                            closeReason = ConnectionCloseReason.Aborted;
                            readLength = -1;
                        }
                        catch (IOException ex) when ((ex.InnerException as SocketException)?.ErrorCode ==
                                                     (int)SocketError.ConnectionReset)
                        {
                            closeReason = ConnectionCloseReason.Reset;
                            readLength = -2;
                        }
                        catch (TaskCanceledException)
                        {
                            if (!m_IsDisposed)
                            {
                                TcpClient.Close();
                                await DisconnectAsync(cancellationToken: cancellationToken);
                            }

                            return;
                        }
                        catch (Exception)
                        {
                            readLength = 0;
                        }

                        if (readLength <= 0)
                        {
                            await OnConnectionClosedAsync(closeReason);
                            TcpClient.Close();
                            return;
                        }

                        try
                        {
                            await OnDataReceivedAsync(new ArraySegment<byte>(buffer, 0, readLength));
                        }
                        catch (Exception ex)
                        {
                            m_Logger.LogError(ex, "{DisplayName} caused exception", DisplayName);
                            await SendMessageAsync(ex.Message, cancellationToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    m_Logger.LogError(ex, "Exception occurred in RCON client task");
                }
            }, cancellationToken);

            return Task.CompletedTask;
        }

        public abstract Task SendMessageAsync(string message, CancellationToken cancellationToken = default);

        public virtual Task PrintMessageAsync(string message)
        {
            return SendMessageAsync(message);
        }

        public virtual Task PrintMessageAsync(string message, Color color)
        {
            return PrintMessageAsync(message);
        }

        public virtual async Task DisconnectAsync(string? reason = null, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(reason))
            {
                try
                {
                    await SendMessageAsync(reason!, cancellationToken);
                }
                catch
                {
                    // ignored
                }
            }

            await OnConnectionClosedAsync(ConnectionCloseReason.Closed);
            await DisposeAsync(cancellationToken);
        }

        protected virtual Task OnConnectionClosedAsync(ConnectionCloseReason connectionCloseReason)
        {
            if (m_IsDisposed || m_Runtime.IsDisposing || !m_Runtime.IsComponentAlive)
            {
                return Task.CompletedTask;
            }

            return m_EventBus.EmitAsync(m_Runtime, this, new RconClientDisconnectedEvent(this, connectionCloseReason));
        }

        protected abstract Task OnDataReceivedAsync(ArraySegment<byte> data);

        protected virtual async Task OnExecuteCommandAsync(string command)
        {
            if (!IsAuthenticated)
            {
                await SendMessageAsync("Login required.");
                return;
            }

            var args = ArgumentsParser.ParseArguments(command);
            await m_CommandExecutor.ExecuteAsync(this, args, string.Empty);
        }

        public virtual async Task SendDataAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            if (m_IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            if (m_Stream != null)
            {
                await m_Stream.WriteAsync(data, offset: 0, data.Length, cancellationToken);
            }
        }

        protected async Task<bool> DisconnectOnFailedLoginAttemptsAsync()
        {
            if (m_FailedAuthAttempts >= 3)
            {
                await DisconnectAsync("Authentication failed.");
                return true;
            }

            return false;
        }

        protected virtual async Task OnAuthenticatingAsync(string username, string password)
        {
            if (await DisconnectOnFailedLoginAttemptsAsync())
            {
                return;
            }

            var accounts = m_OpenModConfiguration.GetSection("rcon:accounts");
            string? matchedAccountId = null;

            foreach (var account in accounts.GetChildren())
            {
                var accountId = account["id"];
                if (!username.Equals(accountId, StringComparison.Ordinal))
                {
                    continue;
                }

                var accountPassword = account["password"];
                if (accountPassword == "ChangeThisToEnableRcon") // lgtm [cs/hardcoded-credentials]
                {
                    // ignore accounts that have the default password
                    continue;
                }

                if (accountPassword == null || accountPassword.Equals(password, StringComparison.Ordinal))
                {
                    matchedAccountId = accountId;
                    break;
                }
            }

            Id = matchedAccountId ?? "Unauthenicated";
            var @event = new RconClientAuthenticatingEvent(this, username, password)
            {
                IsAuthenticated = matchedAccountId != null
            };

            await m_EventBus.EmitAsync(m_Runtime, this, @event);
            IsAuthenticated = @event.IsAuthenticated;

            if (IsAuthenticated)
            {
                Id = matchedAccountId ?? "Anonymous";
                await OnClientAuthenticatedAsync();
            }
            else
            {
                await OnClientAuthenticationFailedAsync();
            }
        }

        protected void IncreaseFailedAuthAttempts()
        {
            m_FailedAuthAttempts++;
        }

        protected virtual Task OnClientAuthenticationFailedAsync()
        {
            IncreaseFailedAuthAttempts();
            return SendMessageAsync("Authentication failed.");
        }

        protected virtual Task OnClientAuthenticatedAsync()
        {
            return SendMessageAsync("Authenticated.");
        }

        public virtual ValueTask DisposeAsync(CancellationToken cancellationToken)
        {
            if (m_IsDisposed)
            {
                return default;
            }

            TcpClient.Dispose();
            m_Stream = null;

            m_IsDisposed = true;
            return default;
        }

        public ValueTask DisposeAsync()
        {
            return DisposeAsync(default);
        }
    }
}